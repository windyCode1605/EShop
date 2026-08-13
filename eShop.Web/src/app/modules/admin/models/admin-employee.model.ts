export interface IAdminEmployee {
  id: number;
  username: string | null;
  email: string | null;
  phoneNumber: string | null;
  creatDate: string | null; // From API Response DateTime
}

export interface IAdminEmployeeQuery {
  pageNumber: number;
  pageSize: number;
  keyword?: string | null;
  isActive?: number | null;
  formDate?: string | null;
  toDate?: string | null;
}
