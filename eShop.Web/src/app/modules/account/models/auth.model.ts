import { ApiResponse, UserDto } from '../../../core/models';

export interface RegisterRequest {
  fullName: string;
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

export interface ForgotPasswordRequest {
  email: string;
}

export interface VerifyResetOtpRequest {
  email: string;
  otpCode: string;
}

export interface VerifyResetOtpResponseData {
  resetToken: string;
}

export interface ResetPasswordRequest {
  email: string;
  resetToken: string;
  newPassword: string;
}

export type ForgotPasswordApiResponse = ApiResponse<any>;
export type VerifyResetOtpApiResponse = ApiResponse<VerifyResetOtpResponseData>;
export type ResetPasswordApiResponse = ApiResponse<any>;
