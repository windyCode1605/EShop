import { ApiResponse, UserDto } from '../../../core/models';

export interface RegisterRequest {
  userName: string;
  email: string;
  userCode: string;
}

export interface VerifyRegisterOtpRequest {
  email: string;
  otpCode: string;
}

export type RegisterApiResponse = ApiResponse<UserDto>;
export type VerifyRegisterOtpApiResponse = ApiResponse<string>;

export interface SetPasswordRequest {
  id: number;
  password: string;
  isPasswordTemp: boolean;
}

export type SetPasswordApiResponse = ApiResponse<string>;
