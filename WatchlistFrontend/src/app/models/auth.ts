// src/app/models/auth.ts

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
  fName: string;
  lName: string;
}

export interface LoginDto {
  usernameOrEmail: string;
  password: string;
}

export interface AuthResponseDto {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: Date;
  refreshTokenExpiresAtUtc: Date;
}

export interface RefreshRequestDto {
  refreshToken: string;
}
