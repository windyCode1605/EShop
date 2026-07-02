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
