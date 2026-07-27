import { Directive, effect, inject, Input, TemplateRef, ViewContainerRef } from "@angular/core";
import { PermissionStore } from "../../core/stores/permission.store";

@Directive({
    selector: '[hasPermission]',
    standalone: true
})
export class HasPermissionDirective {
    private permissionStore = inject(PermissionStore);
    private templateRef = inject(TemplateRef);
    private viewContainer = inject(ViewContainerRef);

    private currentPermission: string | string[] = '';

    @Input() set hasPermission(val: string | string[]) {
        this.currentPermission = val;
        this.updateView();
    }

    constructor() {
        // Lắng nghe Signal permissions từ PermissionStore để tự động cập nhật DOM
        effect(() => {
            // Đọc signal permissions để effect thiết lập dependency tracking
            const _ = this.permissionStore.permissions();
            this.updateView();
        });
    }

    private updateView(): void {
        this.viewContainer.clear();

        if (!this.currentPermission || (Array.isArray(this.currentPermission) && this.currentPermission.length === 0)) {
            return;
        }

        let isGranted = false;

        if (Array.isArray(this.currentPermission)) {
            // Nếu là mảng, cho phép hiển thị khi user có ÍT NHẤT 1 quyền trong danh sách
            isGranted = this.currentPermission.some(p => this.permissionStore.hasPermission(p));
        } else {
            isGranted = this.permissionStore.hasPermission(this.currentPermission);
        }

        if (isGranted) {
            this.viewContainer.createEmbeddedView(this.templateRef);
        }
    }
}