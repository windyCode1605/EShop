export interface SysvarResponseDto {
  id: number;
  grName: string;
  varName: string;
  varValue: string;
  varDesc: string;
}

export interface SysvarUpdateDto {
  varValue: string;
}
