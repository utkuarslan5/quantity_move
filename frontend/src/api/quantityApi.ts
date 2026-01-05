import type { MoveQuantityRequest, MoveQuantityResponse } from './types';
import { apiCall } from './apiClient';

// Quantity API
export const quantityApi = {
  async move(request: MoveQuantityRequest): Promise<MoveQuantityResponse> {
    const response = await apiCall<MoveQuantityResponse>('/quantity/move', {
      method: 'POST',
      body: JSON.stringify(request),
    });

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Move quantity failed');
  }
};











