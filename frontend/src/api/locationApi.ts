import type { ValidateLocationRequest, ValidateLocationResponse } from './types';
import { apiCall } from './apiClient';

// Location API
export const locationApi = {
  async validate(request: ValidateLocationRequest): Promise<ValidateLocationResponse> {
    const response = await apiCall<ValidateLocationResponse>('/location/validate', {
      method: 'POST',
      body: JSON.stringify(request),
    });

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Location validation failed');
  }
};






