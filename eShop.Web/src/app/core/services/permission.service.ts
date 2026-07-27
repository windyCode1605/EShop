import { ReturnStatement } from "@angular/compiler";
import { Injectable, signal } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class PermissionService {
    private userPermissions = signal<string[]>([]);

    setPermissions(permissions: string[]): void {
        this.userPermissions.set(permissions || []);
    }

    getPermissions(): string[] {
        return this.userPermissions();
    }

    hasPermission(permission: string): boolean {
        if (!permission) return true;
        return this.userPermissions().includes(permission);
    }

    // Kiểm tra người dùng có ít nhất 1 quyền trong ds không 
    hasAllPermissions(permission: string[]): boolean {
        if (!permission || permission.length === 0) return true;
        return permission.some((p) => this.hasPermission(p));
    }
    hasAnyPermisson(permissions: string[]): boolean {
        if (!permissions || permissions.length === 0) return true;
        return permissions.some((p) => this.hasPermission(p));
    }

    clearPermissions(): void {
        this.userPermissions.set([]);
    }
}