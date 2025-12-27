import { stockApi } from './stockApi';

// Stock Validation Helper (UI-based validation using stock lookup)
export const stockValidation = {
  /**
   * Validates if sufficient stock is available for a move operation
   * Uses stock lookup data to check availability before calling move endpoint
   */
  async validateStockAvailability(
    itemCode: string,
    location: string,
    requestedQuantity: number,
    lot?: string,
    warehouse?: string,
    site?: string
  ): Promise<{ isValid: boolean; availableQuantity: number; message?: string }> {
    try {
      const stockData = await stockApi.lookup({
        itemCode,
        location,
        lot,
        warehouse,
        site
      });

      // Calculate total available quantity at the location
      const availableQuantity = stockData.items
        .filter(item => {
          // Match location
          if (item.location !== location) return false;
          // Match lot if specified
          if (lot && item.lot !== lot) return false;
          // Match warehouse if specified
          if (warehouse && item.warehouse !== warehouse) return false;
          // Match site if specified
          if (site && item.site !== site) return false;
          return true;
        })
        .reduce((sum, item) => sum + item.quantity, 0);

      const isValid = availableQuantity >= requestedQuantity;

      return {
        isValid,
        availableQuantity,
        message: isValid
          ? undefined
          : `Insufficient stock. Available: ${availableQuantity}, Requested: ${requestedQuantity}`
      };
    } catch (error) {
      return {
        isValid: false,
        availableQuantity: 0,
        message: `Error checking stock availability: ${error instanceof Error ? error.message : 'Unknown error'}`
      };
    }
  }
};




