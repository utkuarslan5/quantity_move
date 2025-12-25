// API Response types
export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresIn: number;
  user?: {
    userId: number;
    username: string;
    fullName?: string;
    email?: string;
  };
}

export interface StockLookupRequest {
  itemCode?: string;
  location?: string;
  warehouse?: string;
  site?: string;
  lot?: string;
}

export interface StockItem {
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

export interface StockLookupResponse {
  items: StockItem[];
}

export interface MoveQuantityRequest {
  itemCode: string;
  sourceLocation: string;
  targetLocation: string;
  quantity: number;
  lot?: string;
  warehouse?: string;
  site?: string;
  userId?: string;
}

export interface MoveQuantityResponse {
  success: boolean;
  transactionId?: string;
  message?: string;
  returnCode?: number;
  rowsAffected?: number;
}

export interface ValidateLocationRequest {
  locationCode: string;
  warehouse?: string;
  site?: string;
}

export interface ValidateLocationResponse {
  isValid: boolean;
  message?: string;
  locationCode?: string;
  locationDescription?: string;
}

