export interface IRole {
  id: number;
  name: string;
  description: string;
  isNew?: boolean;
}

export interface ICreateRoleRequest {
  name: string;
  description: string;
  permissionKeys?: any[];
}

export interface IPermissionItem {
  permissionKey: string;
  displayName: string;
  description: string;
}


export interface IPermissionGroup {
  groupName: string; // The dictionary key (e.g., 'Products', 'Orders')
  permissions: IPermissionItem[];
}
