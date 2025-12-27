import type { StockLookupRequest, StockLookupResponse } from './types';
import { apiCall } from './apiClient';

// Stock API
export const stockApi = {
  async lookup(request: StockLookupRequest): Promise<StockLookupResponse> {
    const response = await apiCall<StockLookupResponse>('/stock/lookup', {
      method: 'POST',
      body: JSON.stringify(request),
    });

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Stock lookup failed');
  },

  async lookupGet(params: {
    itemCode?: string;
    location?: string;
    warehouse?: string;
    site?: string;
    lot?: string;
  }): Promise<StockLookupResponse> {
    const queryParams = new URLSearchParams();
    if (params.itemCode) queryParams.append('itemCode', params.itemCode);
    if (params.location) queryParams.append('location', params.location);
    if (params.warehouse) queryParams.append('warehouse', params.warehouse);
    if (params.site) queryParams.append('site', params.site);
    if (params.lot) queryParams.append('lot', params.lot);

    const response = await apiCall<StockLookupResponse>(
      `/stock/lookup?${queryParams.toString()}`,
      {
        method: 'GET',
      }
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Stock lookup failed');
  }
};



