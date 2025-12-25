// REST API Client for Quantity Move API
// Replaces SOAP service calls with REST API calls using JWT authentication

const API_BASE_URL = '/api';

// Token storage key
const TOKEN_STORAGE_KEY = 'quantity_move_token';
const USER_STORAGE_KEY = 'quantity_move_user';

// API Response types
interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  token: string;
  expiresIn: number;
  user?: {
    userId: number;
    username: string;
    fullName?: string;
    email?: string;
  };
}

interface StockLookupRequest {
  itemCode?: string;
  location?: string;
  warehouse?: string;
  site?: string;
  lot?: string;
}

interface StockItem {
  itemCode: string;
  location?: string;
  warehouse?: string;
  site?: string;
  lot?: string;
  quantity: number;
  expiryDate?: string;
  productionDate?: string;
  priority?: number;
}

interface StockLookupResponse {
  items: StockItem[];
}

interface MoveQuantityRequest {
  itemCode: string;
  sourceLocation: string;
  targetLocation: string;
  quantity: number;
  lot?: string;
  warehouse?: string;
  site?: string;
  userId?: string;
}

interface MoveQuantityMultiRequest {
  itemCode: string;
  sourceLocation: string;
  targets: Array<{
    targetLocation: string;
    quantity: number;
    warehouse?: string;
    site?: string;
  }>;
  totalQuantity: number;
  lot?: string;
  userId?: string;
}

interface MoveQuantityResponse {
  success: boolean;
  transactionId?: string;
  message?: string;
  returnCode?: number;
  rowsAffected?: number;
}

interface ValidateStockRequest {
  itemCode: string;
  location: string;
  quantity: number;
  lot?: string;
  warehouse?: string;
  site?: string;
}

interface ValidateStockResponse {
  isValid: boolean;
  message?: string;
  availableQuantity?: number;
  returnCode?: number;
}

interface ValidateLocationRequest {
  locationCode: string;
  warehouse?: string;
  site?: string;
}

interface ValidateLocationResponse {
  isValid: boolean;
  message?: string;
  locationCode?: string;
  locationDescription?: string;
}

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

// API client with automatic token handling
async function apiCall<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<ApiResponse<T>> {
  const token = tokenManager.getToken();
  
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...options.headers,
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      headers,
    });

    // Handle 401 Unauthorized - token expired or invalid
    if (response.status === 401) {
      tokenManager.removeToken();
      throw new Error('Authentication failed. Please login again.');
    }

    const data: ApiResponse<T> = await response.json();

    if (!response.ok) {
      throw new Error(data.message || `HTTP error! status: ${response.status}`);
    }

    return data;
  } catch (error) {
    console.error('API call failed:', error);
    throw error;
  }
}

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

// Quantity API
export const quantityApi = {
  async validateStock(request: ValidateStockRequest): Promise<ValidateStockResponse> {
    const response = await apiCall<ValidateStockResponse>('/quantity/validate-stock', {
      method: 'POST',
      body: JSON.stringify(request),
    });

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Stock validation failed');
  },

  async move(request: MoveQuantityRequest): Promise<MoveQuantityResponse> {
    const response = await apiCall<MoveQuantityResponse>('/quantity/move', {
      method: 'POST',
      body: JSON.stringify(request),
    });

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Move quantity failed');
  },

  async moveMulti(request: MoveQuantityMultiRequest): Promise<MoveQuantityResponse> {
    const response = await apiCall<MoveQuantityResponse>('/quantity/move-multi', {
      method: 'POST',
      body: JSON.stringify(request),
    });

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Multi-warehouse move failed');
  }
};

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

// Export all APIs
export default {
  auth: authApi,
  stock: stockApi,
  quantity: quantityApi,
  location: locationApi,
  tokenManager,
};

