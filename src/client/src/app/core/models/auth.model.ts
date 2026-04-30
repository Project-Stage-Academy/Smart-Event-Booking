export interface LoginRequest {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName?: string;
}

export interface AuthResponse {
  message: string;
  roles: string[];
}

export interface AuthSession {
  email: string;
  rememberMe: boolean;
  message: string;
  loggedInAt: string;
  roles: string[];
}
