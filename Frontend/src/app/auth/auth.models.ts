/** The signed-in user (matches the API's UserDto). */
export interface AuthUser {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  age: number;
  isGovernmentOfficial: boolean;
  isLawEnforcementOrMilitary: boolean;
}

/** Response from /auth/register and /auth/login. */
export interface AuthResult {
  token: string;
  expiresAtUtc: string;
  user: AuthUser;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  firstName: string;
  lastName: string;
  age: number;
  isGovernmentOfficial: boolean;
  isLawEnforcementOrMilitary: boolean;
  password: string;
}
