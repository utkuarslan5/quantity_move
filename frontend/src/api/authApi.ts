import type { LoginRequest, LoginResponse } from './types';
import { apiCall } from './apiClient';
import { tokenManager } from './tokenManager';

// Authentication API
export const authApi = {
  async login(username: string, password: string): Promise<LoginResponse> {
    const response = await apiCall<LoginResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ username, password } as LoginRequest),
    });

    if (response.success && response.data) {
      tokenManager.setToken(response.data.token);
      if (response.data.user) {
        tokenManager.setUser(response.data.user);
      }
      return response.data;
    }

    throw new Error(response.message || 'Login failed');
  },

  logout(): void {
    tokenManager.removeToken();
  }
};










