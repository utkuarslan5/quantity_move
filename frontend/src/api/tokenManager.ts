import type { LoginResponse } from './types';

// Token storage keys
const TOKEN_STORAGE_KEY = 'quantity_move_token';
const USER_STORAGE_KEY = 'quantity_move_user';

// Token management
export const tokenManager = {
  getToken(): string | null {
    if (typeof window !== 'undefined') {
      return localStorage.getItem(TOKEN_STORAGE_KEY);
    }
    return null;
  },

  setToken(token: string): void {
    if (typeof window !== 'undefined') {
      localStorage.setItem(TOKEN_STORAGE_KEY, token);
    }
  },

  removeToken(): void {
    if (typeof window !== 'undefined') {
      localStorage.removeItem(TOKEN_STORAGE_KEY);
      localStorage.removeItem(USER_STORAGE_KEY);
    }
  },

  getUser(): LoginResponse['user'] | null {
    if (typeof window !== 'undefined') {
      const userStr = localStorage.getItem(USER_STORAGE_KEY);
      return userStr ? JSON.parse(userStr) : null;
    }
    return null;
  },

  setUser(user: LoginResponse['user']): void {
    if (typeof window !== 'undefined') {
      localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user));
    }
  },

  isAuthenticated(): boolean {
    return this.getToken() !== null;
  }
};






