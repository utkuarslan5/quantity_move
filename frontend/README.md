# Frontend Validation Guide

This document explains what validations the UI must perform, where to call them, and how to implement them when building the Quantity Move frontend application.

## Overview

The frontend should perform **client-side validations** before making API calls to provide immediate user feedback and prevent unnecessary server requests. The API client provides validation helpers that use stock lookup data for real-time validation.

## Required Validations

### 1. Stock Availability Validation

**What**: Validate that sufficient stock is available at the source location before attempting a move operation.

**When**: 
- Before calling the `move` API endpoint
- When user changes quantity, item code, or source location
- On form submission (before API call)

**Where**: In the quantity move form component, before the move button is clicked.

**How**: Use `stockValidation.validateStockAvailability()`

### 2. Location Validation

**What**: Validate that source and target locations are valid location codes.

**When**:
- When user enters or changes location codes
- On blur/change events for location input fields
- Before stock validation (locations must be valid first)

**Where**: In location input fields (source and target location).

**How**: Use `locationApi.validate()`

### 3. Form Field Validations

**What**: Basic input validations (required fields, quantity > 0, etc.)

**When**: 
- On input change
- On form submission
- Real-time as user types (optional, for better UX)

**Where**: In form components using standard HTML5 validation or form library validation.

**How**: Standard form validation (HTML5 attributes or validation library)

---

## Implementation Guide

### Import the API Client

```typescript
import apiClient from './api/wsGeneralClient';
// Or import specific functions:
import { stockValidation, locationApi, quantityApi } from './api/wsGeneralClient';
```

### 1. Stock Availability Validation

#### Example: React Component with Stock Validation

```typescript
import { useState, useEffect } from 'react';
import { stockValidation, quantityApi } from './api/wsGeneralClient';

function MoveQuantityForm() {
  const [itemCode, setItemCode] = useState('');
  const [sourceLocation, setSourceLocation] = useState('');
  const [targetLocation, setTargetLocation] = useState('');
  const [quantity, setQuantity] = useState(0);
  const [lot, setLot] = useState('');
  const [warehouse, setWarehouse] = useState('');
  const [site, setSite] = useState('');
  
  const [stockValidationResult, setStockValidationResult] = useState<{
    isValid: boolean;
    availableQuantity: number;
    message?: string;
  } | null>(null);
  
  const [isValidating, setIsValidating] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Validate stock availability when relevant fields change
  useEffect(() => {
    const validateStock = async () => {
      // Only validate if we have minimum required fields
      if (!itemCode || !sourceLocation || quantity <= 0) {
        setStockValidationResult(null);
        return;
      }

      setIsValidating(true);
      setError(null);

      try {
        const result = await stockValidation.validateStockAvailability(
          itemCode,
          sourceLocation,
          quantity,
          lot || undefined,
          warehouse || undefined,
          site || undefined
        );

        setStockValidationResult(result);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Validation failed');
        setStockValidationResult({
          isValid: false,
          availableQuantity: 0,
          message: 'Error validating stock availability'
        });
      } finally {
        setIsValidating(false);
      }
    };

    // Debounce validation to avoid too many API calls
    const timeoutId = setTimeout(validateStock, 500);
    return () => clearTimeout(timeoutId);
  }, [itemCode, sourceLocation, quantity, lot, warehouse, site]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Final validation before submission
    if (!stockValidationResult || !stockValidationResult.isValid) {
      alert('Please ensure sufficient stock is available before submitting.');
      return;
    }

    try {
      const result = await quantityApi.move({
        itemCode,
        sourceLocation,
        targetLocation,
        quantity,
        lot: lot || undefined,
        warehouse: warehouse || undefined,
        site: site || undefined
      });

      if (result.success) {
        alert(`Move successful! Transaction ID: ${result.transactionId}`);
        // Reset form or navigate
      } else {
        alert(`Move failed: ${result.message}`);
      }
    } catch (err) {
      alert(`Error: ${err instanceof Error ? err.message : 'Unknown error'}`);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      {/* Form fields */}
      <input
        type="text"
        value={itemCode}
        onChange={(e) => setItemCode(e.target.value)}
        placeholder="Item Code"
        required
      />
      
      <input
        type="text"
        value={sourceLocation}
        onChange={(e) => setSourceLocation(e.target.value)}
        placeholder="Source Location"
        required
      />
      
      <input
        type="number"
        value={quantity}
        onChange={(e) => setQuantity(parseFloat(e.target.value))}
        placeholder="Quantity"
        min="0.01"
        step="0.01"
        required
      />

      {/* Stock validation feedback */}
      {isValidating && <p>Validating stock availability...</p>}
      
      {stockValidationResult && (
        <div>
          {stockValidationResult.isValid ? (
            <p style={{ color: 'green' }}>
              ✓ Stock available: {stockValidationResult.availableQuantity} units
            </p>
          ) : (
            <p style={{ color: 'red' }}>
              ✗ {stockValidationResult.message}
            </p>
          )}
        </div>
      )}

      {error && <p style={{ color: 'red' }}>Error: {error}</p>}

      <button 
        type="submit"
        disabled={!stockValidationResult?.isValid || isValidating}
      >
        Move Quantity
      </button>
    </form>
  );
}
```

#### Function Signature

```typescript
stockValidation.validateStockAvailability(
  itemCode: string,
  location: string,
  requestedQuantity: number,
  lot?: string,
  warehouse?: string,
  site?: string
): Promise<{
  isValid: boolean;
  availableQuantity: number;
  message?: string;
}>
```

#### Return Value

- `isValid`: `true` if available quantity >= requested quantity
- `availableQuantity`: Total available quantity at the location (after filtering)
- `message`: Error message if validation fails, `undefined` if valid

---

### 2. Location Validation

#### Example: Validate Location on Input Change

```typescript
import { useState } from 'react';
import { locationApi } from './api/wsGeneralClient';

function LocationInput({ 
  value, 
  onChange, 
  label,
  warehouse,
  site 
}: {
  value: string;
  onChange: (value: string) => void;
  label: string;
  warehouse?: string;
  site?: string;
}) {
  const [isValidating, setIsValidating] = useState(false);
  const [validationResult, setValidationResult] = useState<{
    isValid: boolean;
    message?: string;
  } | null>(null);

  const handleBlur = async () => {
    if (!value.trim()) {
      setValidationResult(null);
      return;
    }

    setIsValidating(true);
    try {
      const result = await locationApi.validate({
        locationCode: value,
        warehouse: warehouse || undefined,
        site: site || undefined
      });

      setValidationResult({
        isValid: result.isValid,
        message: result.message
      });
    } catch (err) {
      setValidationResult({
        isValid: false,
        message: err instanceof Error ? err.message : 'Validation failed'
      });
    } finally {
      setIsValidating(false);
    }
  };

  return (
    <div>
      <label>{label}</label>
      <input
        type="text"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        onBlur={handleBlur}
        required
      />
      
      {isValidating && <span>Validating...</span>}
      
      {validationResult && (
        <span style={{ 
          color: validationResult.isValid ? 'green' : 'red' 
        }}>
          {validationResult.isValid ? '✓ Valid' : `✗ ${validationResult.message}`}
        </span>
      )}
    </div>
  );
}
```

#### Function Signature

```typescript
locationApi.validate(request: {
  locationCode: string;
  warehouse?: string;
  site?: string;
}): Promise<ValidateLocationResponse>
```

---

### 3. Form Field Validations

#### Example: HTML5 Validation + Custom Validation

```typescript
function MoveQuantityForm() {
  const [formData, setFormData] = useState({
    itemCode: '',
    sourceLocation: '',
    targetLocation: '',
    quantity: 0
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    // Required field validations
    if (!formData.itemCode.trim()) {
      newErrors.itemCode = 'Item code is required';
    }

    if (!formData.sourceLocation.trim()) {
      newErrors.sourceLocation = 'Source location is required';
    }

    if (!formData.targetLocation.trim()) {
      newErrors.targetLocation = 'Target location is required';
    }

    // Quantity validation
    if (formData.quantity <= 0) {
      newErrors.quantity = 'Quantity must be greater than 0';
    }

    // Source and target must be different
    if (formData.sourceLocation === formData.targetLocation) {
      newErrors.targetLocation = 'Target location must be different from source';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    // Additional validations (stock, location) would go here
    // ...
  };

  return (
    <form onSubmit={handleSubmit} noValidate>
      <div>
        <label>Item Code *</label>
        <input
          type="text"
          value={formData.itemCode}
          onChange={(e) => setFormData({ ...formData, itemCode: e.target.value })}
          required
        />
        {errors.itemCode && <span style={{ color: 'red' }}>{errors.itemCode}</span>}
      </div>

      <div>
        <label>Quantity *</label>
        <input
          type="number"
          value={formData.quantity}
          onChange={(e) => setFormData({ ...formData, quantity: parseFloat(e.target.value) })}
          min="0.01"
          step="0.01"
          required
        />
        {errors.quantity && <span style={{ color: 'red' }}>{errors.quantity}</span>}
      </div>

      {/* Other fields... */}

      <button type="submit">Move Quantity</button>
    </form>
  );
}
```

---

## Validation Workflow

### Recommended Flow for Move Quantity Operation

```
1. User enters form data
   ↓
2. Basic form validation (required fields, quantity > 0, etc.)
   ↓
3. Location validation (validate source and target locations)
   ↓
4. Stock availability validation (using stockValidation helper)
   ↓
5. If all validations pass → Call quantityApi.move()
   ↓
6. Handle response (success/error)
```

### Complete Example: Full Validation Flow

```typescript
import { useState } from 'react';
import { stockValidation, locationApi, quantityApi } from './api/wsGeneralClient';

async function handleMoveQuantity(formData: {
  itemCode: string;
  sourceLocation: string;
  targetLocation: string;
  quantity: number;
  lot?: string;
  warehouse?: string;
  site?: string;
}) {
  // Step 1: Basic form validation
  if (!formData.itemCode || !formData.sourceLocation || !formData.targetLocation) {
    throw new Error('All required fields must be filled');
  }

  if (formData.quantity <= 0) {
    throw new Error('Quantity must be greater than 0');
  }

  if (formData.sourceLocation === formData.targetLocation) {
    throw new Error('Source and target locations must be different');
  }

  // Step 2: Validate locations
  const [sourceLocationValid, targetLocationValid] = await Promise.all([
    locationApi.validate({ locationCode: formData.sourceLocation }),
    locationApi.validate({ locationCode: formData.targetLocation })
  ]);

  if (!sourceLocationValid.isValid) {
    throw new Error(`Invalid source location: ${sourceLocationValid.message}`);
  }

  if (!targetLocationValid.isValid) {
    throw new Error(`Invalid target location: ${targetLocationValid.message}`);
  }

  // Step 3: Validate stock availability
  const stockValidationResult = await stockValidation.validateStockAvailability(
    formData.itemCode,
    formData.sourceLocation,
    formData.quantity,
    formData.lot,
    formData.warehouse,
    formData.site
  );

  if (!stockValidationResult.isValid) {
    throw new Error(stockValidationResult.message || 'Insufficient stock');
  }

  // Step 4: All validations passed, proceed with move
  const result = await quantityApi.move({
    itemCode: formData.itemCode,
    sourceLocation: formData.sourceLocation,
    targetLocation: formData.targetLocation,
    quantity: formData.quantity,
    lot: formData.lot,
    warehouse: formData.warehouse,
    site: formData.site
  });

  return result;
}
```

---

## Best Practices

### 1. Debounce Validation Calls

Avoid making validation API calls on every keystroke. Use debouncing:

```typescript
import { useEffect, useRef } from 'react';

function useDebounce<T>(value: T, delay: number): T {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return debouncedValue;
}

// Usage
const debouncedQuantity = useDebounce(quantity, 500);
```

### 2. Show Loading States

Always show loading indicators during validation:

```typescript
{isValidating && <span>Validating...</span>}
```

### 3. Provide Clear Error Messages

Display validation errors clearly to users:

```typescript
{validationResult && !validationResult.isValid && (
  <div style={{ color: 'red' }}>
    {validationResult.message}
  </div>
)}
```

### 4. Disable Submit Button During Validation

Prevent form submission while validation is in progress:

```typescript
<button 
  type="submit" 
  disabled={isValidating || !validationResult?.isValid}
>
  Move Quantity
</button>
```

### 5. Handle Errors Gracefully

Wrap validation calls in try-catch blocks:

```typescript
try {
  const result = await stockValidation.validateStockAvailability(...);
} catch (error) {
  console.error('Validation error:', error);
  // Show user-friendly error message
  setError('Unable to validate stock. Please try again.');
}
```

---

## Validation Timing Summary

| Validation | When to Call | Where to Call |
|------------|--------------|---------------|
| **Form Fields** | On input change, on blur, on submit | Form component |
| **Location** | On location input blur/change | Location input component |
| **Stock Availability** | When item/location/quantity changes, before submit | Move quantity form component |
| **Final Check** | On form submit (before API call) | Form submit handler |

---

## Notes

- **UI validation is for user experience** - The server-side stored procedure still performs final validation
- **Race conditions are possible** - Stock may change between UI validation and API call
- **Always handle API errors** - Even if UI validation passes, the API call may fail
- **Refresh stock data** - Consider refreshing stock lookup data before validation if data might be stale
- **Token management** - Ensure user is authenticated before making validation calls (token is automatically included)

---

## API Client Reference

For complete API documentation, see `src/api/wsGeneralClient.ts`. Available exports:

- `stockValidation.validateStockAvailability()` - Stock availability validation
- `locationApi.validate()` - Location code validation
- `quantityApi.move()` - Execute quantity move
- `stockApi.lookup()` - Get stock data
- `authApi.login()` - User authentication
- `tokenManager` - Token management utilities

