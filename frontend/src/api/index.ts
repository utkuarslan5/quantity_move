// Main API client exports
export { authApi } from './authApi';
export { stockApi } from './stockApi';
export { stockValidation } from './stockValidation';
export { quantityApi } from './quantityApi';
export { locationApi } from './locationApi';
export { tokenManager } from './tokenManager';
export { apiCall } from './apiClient';

// Type exports
export type * from './types';

// Default export for convenience
import { authApi } from './authApi';
import { stockApi } from './stockApi';
import { stockValidation } from './stockValidation';
import { quantityApi } from './quantityApi';
import { locationApi } from './locationApi';
import { tokenManager } from './tokenManager';

export default {
  auth: authApi,
  stock: stockApi,
  stockValidation: stockValidation,
  quantity: quantityApi,
  location: locationApi,
  tokenManager,
};











