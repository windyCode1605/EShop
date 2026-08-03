import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminRoleService } from '../../services/admin-role.service';
import { IRole, IPermissionItem, IPermissionGroup } from '../../models/admin-role.model';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { finalize, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { PERMISSIONS } from '../../../../core/constants/permissions.const';
import { ToastService } from '../../../../core/services/toast.service';


const GROUP_ACCENT: Record<string, string> = {};
const GROUP_PALETTE = ['#6366F1', '#F59E0B', '#10B981', '#F97316', '#EC4899', '#8B5CF6', '#06B6D4', '#EF4444'];

@Component({
  selector: 'app-admin-roles',
  standalone: true,
  imports: [CommonModule, FormsModule, ToastModule, HasPermissionDirective],
  styles: [`
    :host { display: block; }

    /* Custom Toggle */
    .toggle-wrap { position: relative; display: inline-flex; align-items: center; cursor: pointer; }
    .toggle-track {
      width: 40px; height: 22px; border-radius: 999px;
      transition: background 200ms; position: relative;
    }
    .toggle-track.on  { background: #0EA5E9; }
    .toggle-track.off { background: var(--admin-input-border); }
    .toggle-dot {
      position: absolute; top: 3px;
      width: 16px; height: 16px; border-radius: 50%; background: #fff;
      transition: transform 200ms cubic-bezier(.4,0,.2,1);
      box-shadow: 0 1px 3px rgba(0,0,0,.4);
    }
    .toggle-track.on  .toggle-dot { transform: translateX(18px); }
    .toggle-track.off .toggle-dot { transform: translateX(3px); }

    /* Table rows */
    .perm-row { border-bottom: 1px solid var(--admin-border-sub); transition: background 150ms; }
    .perm-row:hover { background: var(--admin-table-row-hover); }
    .perm-row:last-child { border-bottom: none; }

    /* Search */
    .search-input::placeholder { color: var(--admin-text-muted); }
    .search-input:focus { outline: none; border-color: rgba(99,102,241,0.5); box-shadow: 0 0 0 1px rgba(99,102,241,0.3); }

    /* Dropdown */
    select.dark-select {
      appearance: none;
      background: var(--admin-input-bg);
      border: 1px solid var(--admin-input-border); border-radius: 10px;
      color: var(--admin-text-secondary); padding: 0 36px 0 12px; height: 38px; font-size: 13px;
      cursor: pointer;
      background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='%2371717A' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpath d='m6 9 6 6 6-6'/%3E%3C/svg%3E");
      background-repeat: no-repeat;
      background-position: right 10px center;
    }
    select.dark-select:focus { outline: none; border-color: rgba(99,102,241,0.5); }

    /* Scrollbar */
    ::-webkit-scrollbar { width: 4px; height: 4px; }
    ::-webkit-scrollbar-track { background: transparent; }
    ::-webkit-scrollbar-thumb { background: var(--admin-border); border-radius: 99px; }
  `],
  template: `
    <p-toast position="bottom-right"></p-toast>

    <div class="min-h-screen w-full" style="background:var(--admin-canvas); color:var(--admin-text-primary);">

      <!-- ── Page Content ────────────────────────────────────────── -->
      <div class="px-8 py-8 max-w-[1280px] mx-auto">

        <!-- Header -->
        <header class="mb-8">
          <h1 class="text-[28px] font-semibold tracking-tight mb-1.5" style="color:var(--admin-text-primary);">Permissions Matrix</h1>
          <p class="text-sm" style="color:var(--admin-text-secondary);">Manage granular user access and role definitions</p>
        </header>

        <!-- Filter Bar -->
        <div class="flex items-center gap-3 mb-6 flex-wrap">
          <!-- Search -->
          <div class="relative flex-1 min-w-[240px] max-w-[340px]">
            <svg class="absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#52525B" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
            <input
              type="text"
              [(ngModel)]="searchQuery"
              placeholder="Search permissions..."
              class="search-input w-full pl-9 pr-4 h-[38px] rounded-[10px] text-[13px]"
              style="background:var(--admin-input-bg); border:1px solid var(--admin-input-border); color:var(--admin-text-primary);" />
          </div>

          <!-- Role Filter -->
          <select class="dark-select" [(ngModel)]="selectedFilterRoleId">
            <option [ngValue]="null">All Roles</option>
            <option *ngFor="let r of roles()" [ngValue]="r.id">{{ r.name }}</option>
          </select>

          <!-- Module Filter -->
          <select class="dark-select" [(ngModel)]="selectedModule">
            <option value="">Active Modules</option>
            <option *ngFor="let g of allPermissionsGrouped()" [value]="g.groupName">{{ g.groupName }}</option>
          </select>

          <div class="flex-1"></div>

          <!-- Create Role -->
          <button *hasPermission="PERMISSIONS.IDENTITY.ROLES_CREATE" (click)="openCreateRole()" class="h-[38px] px-4 rounded-[10px] text-[13px] font-medium flex items-center gap-2 transition-colors"
                  style="border:1px solid var(--admin-btn-border); background:var(--admin-btn-bg); color:var(--admin-btn-text);">
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="M12 5v14"/></svg>
            New Role
          </button>

          <!-- Apply Button -->
          <button *hasPermission="[PERMISSIONS.IDENTITY.ROLES_MANAGE, PERMISSIONS.IDENTITY.ROLES_VIEW]" (click)="saveAllChanges()" [disabled]="saving() || dirtyRoles().size === 0"
                  class="h-[38px] px-5 rounded-[10px] text-[13px] font-semibold transition-all active:scale-95 disabled:opacity-40 flex items-center gap-2"
                  style="background:var(--admin-text-primary); color:var(--admin-canvas);">
            <svg *ngIf="saving()" class="animate-spin" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10" opacity=".25"/><path d="M12 2a10 10 0 0 1 10 10" opacity=".75"/></svg>
            Apply Changes
            <span *ngIf="dirtyRoles().size > 0" class="ml-1 text-[11px] px-1.5 py-0.5 rounded-full" style="background:rgba(0,0,0,0.15);">{{ dirtyRoles().size }}</span>
          </button>
        </div>


        <!-- Loading State -->
        <div *ngIf="initialLoading()" class="flex flex-col gap-3">
          <div *ngFor="let s of [1,2,3]" class="animate-pulse rounded-2xl h-[52px]" style="background:var(--admin-surface); border:1px solid var(--admin-border);"></div>
        </div>

        <!-- Permissions Table -->
        <div *ngIf="!initialLoading()" class="rounded-2xl overflow-hidden" style="border:1px solid var(--admin-border);">

          <!-- Column Headers -->
          <div class="grid items-center px-4 py-3 text-[11px] font-semibold uppercase tracking-widest select-none"
               [style.grid-template-columns]="columnTemplate()"
               style="background:var(--admin-table-header); color:var(--admin-text-muted); border-bottom:1px solid var(--admin-border);">
            <div class="flex items-center">
              <input type="checkbox" (change)="toggleSelectAll($event)" class="w-4 h-4 rounded" style="accent-color:#6366F1;" />
            </div>
            <div>Permission Key</div>
            <div>Display Name</div>
            <div class="hidden xl:block">Description</div>
            <div *ngFor="let r of visibleRoles()" class="text-center truncate">{{ r.name }}</div>
          </div>

          <!-- Groups -->
          <ng-container *ngFor="let group of filteredGroups(); let gi = index">

            <!-- Group Header -->
            <div class="flex items-center gap-3 px-4 h-[52px] cursor-pointer select-none group transition-colors"
                 [style.background]="expandedGroups().has(group.groupName) ? 'var(--admin-surface)' : 'var(--admin-canvas)'"
                 style="border-bottom:1px solid var(--admin-border-sub);"
                 (click)="toggleGroup(group.groupName)">
              <!-- Accent Bar -->
              <div class="w-[3px] h-7 rounded-full shrink-0" [style.background]="getGroupAccent(group.groupName, gi)"></div>
              <!-- Icon -->
              <div class="w-6 h-6 flex items-center justify-center shrink-0" [style.color]="getGroupAccent(group.groupName, gi)">
                <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect width="18" height="18" x="3" y="3" rx="2"/><path d="M7 7h10M7 12h10M7 17h10"/></svg>
              </div>
              <span class="text-[12px] font-semibold uppercase tracking-widest" style="color:var(--admin-text-primary);">{{ group.groupName }}</span>
              <span class="text-[11px] ml-1" style="color:var(--admin-text-muted);">({{ group.permissions.length }})</span>
              <div class="flex-1"></div>
              <svg [class.rotate-180]="expandedGroups().has(group.groupName)" class="transition-transform duration-200" width="14" height="14" viewBox="0 0 24 24" fill="none" [attr.stroke]="'var(--admin-text-muted)'" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m6 9 6 6 6-6"/></svg>
            </div>

            <!-- Permission Rows (expanded) -->
            <ng-container *ngIf="expandedGroups().has(group.groupName)">
              <div *ngFor="let perm of filteredPerms(group); let pi = index"
                   class="perm-row grid items-center px-4 h-[52px]"
                   [style.grid-template-columns]="columnTemplate()"
                   [style.background]="pi % 2 === 0 ? 'var(--admin-table-row-odd)' : 'var(--admin-table-row-even)'">

                <!-- Checkbox -->
                <div>
                  <input type="checkbox"
                         [checked]="isPermChecked(perm.permissionKey)"
                         (change)="togglePermRow($event, perm.permissionKey)"
                         class="w-4 h-4 rounded"
                         style="accent-color:#6366F1;" />
                </div>

                <!-- Permission Key (monospace, sky) -->
                <div class="font-mono text-[12px] truncate pr-2" style="color:#38BDF8;">{{ perm.permissionKey }}</div>

                <!-- Display Name -->
                <div class="text-[13px] truncate pr-2" style="color:var(--admin-text-primary);">{{ perm.displayName }}</div>

                <!-- Description (hidden on smaller screens) -->
                <div class="hidden xl:block text-[12px] truncate pr-4 leading-snug" style="color:var(--admin-text-muted);" [title]="perm.description">{{ perm.description }}</div>

                <!-- Role Toggles -->
                <div *ngFor="let role of visibleRoles()" class="flex justify-center">
                  <span class="toggle-wrap" (click)="togglePermForRole(role.id, perm.permissionKey)">
                    <span class="toggle-track" [class.on]="hasRolePerm(role.id, perm.permissionKey)" [class.off]="!hasRolePerm(role.id, perm.permissionKey)">
                      <span class="toggle-dot"></span>
                    </span>
                  </span>
                </div>

              </div>
            </ng-container>

          </ng-container>

          <!-- Empty search state -->
          <div *ngIf="filteredGroups().length === 0 && !initialLoading()" class="flex flex-col items-center justify-center py-16" style="background:var(--admin-surface);">
            <svg width="32" height="32" viewBox="0 0 24 24" fill="none" [attr.stroke]="'var(--admin-border)'" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="mb-4"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
            <p class="text-sm" style="color:var(--admin-text-secondary);">No permissions match your search.</p>
          </div>

        </div>

      </div><!-- /max-w -->
    </div><!-- /page -->

    <!-- ── Create Role Modal ────────────────────────────────────── -->
    <div *ngIf="showCreateModal"
         class="fixed inset-0 z-50 flex items-center justify-center p-4"
         style="background:rgba(0,0,0,0.7); backdrop-filter:blur(6px);">
      <div class="w-full max-w-[460px] rounded-3xl p-8 shadow-2xl"
           style="background:var(--admin-modal-bg); border:1px solid var(--admin-modal-border);">
        <h3 class="text-xl font-semibold mb-1.5" style="color:var(--admin-text-primary);">New Role</h3>
        <p class="text-sm mb-6" style="color:var(--admin-text-secondary);">Define a new role with a name and optional description.</p>

        <div class="flex flex-col gap-4">
          <div>
            <label class="block text-[12px] font-medium mb-2 pl-0.5" style="color:var(--admin-text-secondary);">Role Name *</label>
            <input type="text" [(ngModel)]="newRoleData.name" placeholder="e.g. Editor"
                   class="search-input w-full px-4 h-[42px] rounded-[12px] text-[14px]"
                   style="background:var(--admin-canvas); border:1px solid var(--admin-input-border); color:var(--admin-text-primary);" />
          </div>
          <div>
            <label class="block text-[12px] font-medium mb-2 pl-0.5" style="color:var(--admin-text-secondary);">Description</label>
            <textarea [(ngModel)]="newRoleData.description" rows="3" placeholder="Describe what this role can do..."
                      class="search-input w-full px-4 py-3 rounded-[12px] text-[14px] resize-none"
                      style="background:var(--admin-canvas); border:1px solid var(--admin-input-border); color:var(--admin-text-primary);"></textarea>
          </div>
        </div>

        <div class="mt-6 flex justify-end gap-3">
          <button (click)="closeCreateRole()"
                  class="px-5 h-[38px] rounded-[10px] text-[13px] font-medium transition-colors"
                  style="color:var(--admin-text-secondary); background:transparent;">Cancel</button>
          <button (click)="submitCreateRole()"
                  [disabled]="!newRoleData.name.trim()"
                  class="px-5 h-[38px] rounded-[10px] text-[13px] font-semibold transition-all active:scale-95 disabled:opacity-40"
                  style="background:var(--admin-text-primary); color:var(--admin-canvas);">Create Role</button>
        </div>
      </div>
    </div>
  `
})
export class AdminRolesComponent implements OnInit {
  private roleService = inject(AdminRoleService);
  private messageService = inject(MessageService);
  private toastService = inject(ToastService);

  readonly PERMISSIONS = PERMISSIONS;

  // ── Signals ──────────────────────────────────────────────────
  roles = this.roleService.roles;
  allPermissionsGrouped = this.roleService.allPermissionsGrouped;
  initialLoading = signal(false);
  saving = signal(false);

  // Per-role permission map: { [roleId]: Set<permissionKey> }
  rolePermMap = signal<Map<number, Set<string>>>(new Map());

  // Track which role IDs have unsaved changes
  dirtyRoles = signal<Set<number>>(new Set());

  // Expand/collapse state per group
  expandedGroups = signal<Set<string>>(new Set());

  // Filter state
  searchQuery = '';
  selectedFilterRoleId: number | null = null;
  selectedModule = '';

  // Create modal
  showCreateModal = false;
  newRoleData = { name: '', description: '' };

  // ── Computed ──────────────────────────────────────────────────
  /** Roles visible in the matrix columns. If filter is set, only that role */
  visibleRoles = computed(() => {
    if (this.selectedFilterRoleId !== null) {
      return this.roles().filter(r => r.id === this.selectedFilterRoleId);
    }
    return this.roles();
  });

  /** Column template: checkbox + key + name + desc + N role toggles */
  columnTemplate = computed(() => {
    const roleCount = this.visibleRoles().length;
    const roleCols = roleCount > 0 ? Array(roleCount).fill('72px').join(' ') : '';
    return `32px 180px 180px 1fr ${roleCols}`;
  });

  /** Filtered groups based on search + module filter */
  filteredGroups = computed(() => {
    const q = this.searchQuery.trim().toLowerCase();
    const mod = this.selectedModule;
    return this.allPermissionsGrouped()
      .filter(g => !mod || g.groupName === mod)
      .map(g => ({
        ...g,
        permissions: g.permissions.filter(p =>
          !q ||
          p.permissionKey.toLowerCase().includes(q) ||
          p.displayName.toLowerCase().includes(q) ||
          (p.description || '').toLowerCase().includes(q)
        )
      }))
      .filter(g => g.permissions.length > 0);
  });

  //  Lifecycle 
  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.initialLoading.set(true);
    forkJoin([
      this.roleService.fetchRoles(),
      this.roleService.fetchAllPermissions()
    ]).subscribe({
      next: ([roles]) => {
        // Expand first group by default
        const groups = this.roleService.allPermissionsGrouped();
        if (groups.length > 0 && this.expandedGroups().size === 0) {
          this.expandedGroups.set(new Set([groups[0].groupName]));
        }
        // Load permissions for each role
        this.loadAllRolePermissions(roles.map(r => r.id));
      },
      error: (err: any) => {
        this.initialLoading.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.message });
      }
    });
  }

  private loadAllRolePermissions(roleIds: number[]) {
    if (roleIds.length === 0) {
      this.initialLoading.set(false);
      return;
    }

    let completed = 0;
    const newMap = new Map<number, Set<string>>();

    roleIds.forEach(roleId => {
      this.roleService.getRolePermissions(roleId).pipe(
        finalize(() => {
          completed++;
          if (completed === roleIds.length) {
            this.rolePermMap.set(newMap);
            this.initialLoading.set(false);
          }
        })
      ).subscribe({
        next: (keys) => { newMap.set(roleId, new Set(keys)); },
        error: () => { newMap.set(roleId, new Set()); }
      });
    });
  }

  // ── Helpers ───────────────────────────────────────────────────
  hasRolePerm(roleId: number, permKey: string): boolean {
    return this.rolePermMap().get(roleId)?.has(permKey) ?? false;
  }

  isPermChecked(permKey: string): boolean {
    // A perm row checkbox is "checked" if ALL visible roles have it
    return this.visibleRoles().length > 0 &&
      this.visibleRoles().every(r => this.hasRolePerm(r.id, permKey));
  }

  getGroupAccent(groupName: string, index: number): string {
    if (!GROUP_ACCENT[groupName]) {
      GROUP_ACCENT[groupName] = GROUP_PALETTE[index % GROUP_PALETTE.length];
    }
    return GROUP_ACCENT[groupName];
  }

  filteredPerms(group: IPermissionGroup): IPermissionItem[] {
    // find the matching filtered group to get filtered perms
    return this.filteredGroups().find(g => g.groupName === group.groupName)?.permissions ?? [];
  }

  // ── Interactions ──────────────────────────────────────────────
  toggleGroup(groupName: string) {
    const cur = new Set(this.expandedGroups());
    if (cur.has(groupName)) { cur.delete(groupName); } else { cur.add(groupName); }
    this.expandedGroups.set(cur);
  }

  togglePermForRole(roleId: number, permKey: string) {
    const newMap = new Map(this.rolePermMap());
    const perms = new Set(newMap.get(roleId) ?? []);
    if (perms.has(permKey)) { perms.delete(permKey); } else { perms.add(permKey); }
    newMap.set(roleId, perms);
    this.rolePermMap.set(newMap);

    // Mark role as dirty
    const dirty = new Set(this.dirtyRoles());
    dirty.add(roleId);
    this.dirtyRoles.set(dirty);
  }

  togglePermRow(event: Event, permKey: string) {
    const checked = (event.target as HTMLInputElement).checked;
    this.visibleRoles().forEach(r => {
      const newMap = new Map(this.rolePermMap());
      const perms = new Set(newMap.get(r.id) ?? []);
      if (checked) { perms.add(permKey); } else { perms.delete(permKey); }
      newMap.set(r.id, perms);
      this.rolePermMap.set(newMap);
      const dirty = new Set(this.dirtyRoles());
      dirty.add(r.id);
      this.dirtyRoles.set(dirty);
    });
  }

  toggleSelectAll(event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    const allKeys = this.allPermissionsGrouped().flatMap(g => g.permissions.map(p => p.permissionKey));
    this.visibleRoles().forEach(r => {
      const newMap = new Map(this.rolePermMap());
      newMap.set(r.id, new Set(checked ? allKeys : []));
      this.rolePermMap.set(newMap);
      const dirty = new Set(this.dirtyRoles());
      dirty.add(r.id);
      this.dirtyRoles.set(dirty);
    });
  }

  // ── Save ──────────────────────────────────────────────────────
  saveAllChanges() {
    const dirtyIds = Array.from(this.dirtyRoles());
    if (dirtyIds.length === 0) return;

    this.saving.set(true);
    let completed = 0;
    let hasError = false;

    dirtyIds.forEach(roleId => {
      const keys = Array.from(this.rolePermMap().get(roleId) ?? []);
      const role = this.roles().find(r => r.id === roleId);

      let obs$: any;
      if (role?.isNew) {
        // Backend expects an array of objects for CreateRoleRequest
        const allPerms = this.allPermissionsGrouped().flatMap(g => g.permissions);
        const permObjects = keys.map(k => {
          const p = allPerms.find(x => x.permissionKey === k);
          return {
            permissionKey: k,
            displayName: p?.displayName || k,
            description: p?.description || ''
          };
        });

        obs$ = this.roleService.createRole({ name: role.name, description: role.description, permissionKeys: permObjects }).pipe(
          map(newRole => {
            // Swap tempId with real DB ID
            this.roleService.roles.update(r => [...r.filter(x => x.id !== roleId), newRole]);
            const newMap = new Map(this.rolePermMap());
            newMap.set(newRole.id, newMap.get(roleId) ?? new Set());
            newMap.delete(roleId);
            this.rolePermMap.set(newMap);
            return newRole;
          })
        );
      } else {
        obs$ = this.roleService.updateRolePermissions(roleId, keys);
      }

      obs$.pipe(
        finalize(() => {
          completed++;
          if (completed === dirtyIds.length) {
            this.saving.set(false);
            if (!hasError) {
              this.dirtyRoles.set(new Set());
              this.toastService.success('Cập nhật quyền thành công', 'Thành công', { position: 'bottom-right' });
            }
          }
        })
      ).subscribe({
        error: (err: any) => {
          hasError = true;
          this.toastService.error(err.message || 'Cập nhật quyền thất bại', 'Thất bại', { position: 'bottom-right' });
        }
      });
    });
  }

  // ── Create Role ───────────────────────────────────────────────
  openCreateRole() { this.newRoleData = { name: '', description: '' }; this.showCreateModal = true; }
  closeCreateRole() { this.showCreateModal = false; }

  submitCreateRole() {
    const tempId = -Date.now();
    const newRole: IRole = { id: tempId, name: this.newRoleData.name, description: this.newRoleData.description, isNew: true };

    // Add draft role to list
    this.roleService.roles.update(r => [...r, newRole]);

    // Initialize permissions set for draft
    const newMap = new Map(this.rolePermMap());
    newMap.set(tempId, new Set());
    this.rolePermMap.set(newMap);

    // Mark as dirty so it will be saved on Apply Changes
    const dirty = new Set(this.dirtyRoles());
    dirty.add(tempId);
    this.dirtyRoles.set(dirty);

    this.messageService.add({ severity: 'info', summary: 'Draft Created', detail: `Assign permissions to "${newRole.name}" and click Apply Changes to save.` });
    this.closeCreateRole();
  }
}
