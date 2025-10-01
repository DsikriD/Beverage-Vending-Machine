import { createSelector } from '@reduxjs/toolkit';
import { RootState } from '../../../store';

// Базовые селекторы
export const selectCatalogState = (state: RootState) => state.catalog;

export const selectProducts = createSelector(
  [selectCatalogState],
  (catalog) => catalog.products
);

export const selectBrands = createSelector(
  [selectCatalogState],
  (catalog) => catalog.brands
);

export const selectSelectedProducts = createSelector(
  [selectCatalogState],
  (catalog) => catalog.selectedProducts
);

export const selectFilters = createSelector(
  [selectCatalogState],
  (catalog) => catalog.filters
);

export const selectLoading = createSelector(
  [selectCatalogState],
  (catalog) => catalog.loading
);

export const selectError = createSelector(
  [selectCatalogState],
  (catalog) => catalog.error
);

export const selectPagination = createSelector(
  [selectCatalogState],
  (catalog) => catalog.pagination
);

// Селекторы с вычислениями
export const selectAvailableProducts = createSelector(
  [selectProducts],
  (products) => products.filter(product => product.isAvailable && product.stockQuantity > 0)
);

export const selectSelectedProductsCount = createSelector(
  [selectSelectedProducts],
  (selectedProducts) => selectedProducts.length
);

export const selectImportLoading = (state: RootState) => state.catalog.importLoading;
export const selectImportError = (state: RootState) => state.catalog.importError;

export const selectSelectedProductsData = createSelector(
  [selectProducts, selectSelectedProducts],
  (products, selectedIds) => products.filter(product => selectedIds.includes(product.id.toString()))
);

export const selectFilteredProducts = createSelector(
  [selectProducts, selectFilters],
  (products, filters) => {
    let filtered = products;

    // Фильтр по бренду применяется на сервере, но дублируем здесь для надежности
    if (filters.brand && filters.brand !== 'all') {
      filtered = filtered.filter(product => product.brand === filters.brand);
    }

    // Фильтр по цене применяется только на клиенте
    if (filters.maxPrice) {
      filtered = filtered.filter(product => product.price <= filters.maxPrice!);
    }

    // Фильтр по доступности применяется на сервере
    if (filters.available !== undefined) {
      filtered = filtered.filter(product => product.isAvailable === filters.available);
    }

    return filtered;
  }
);

export const selectProductsByBrand = createSelector(
  [selectProducts, (state: RootState, brand: string) => brand],
  (products, brand) => products.filter(product => product.brand === brand)
);

export const selectPriceRange = createSelector(
  [selectProducts],
  (products) => {
    if (products.length === 0) return { min: 0, max: 100 };
    
    const prices = products.map(p => p.price);
    return {
      min: 0, // Всегда начинаем с 0
      max: Math.max(...prices)
    };
  }
);

export const selectPriceRangeByBrand = createSelector(
  [selectProducts, (state: RootState, brand: string) => brand],
  (products, brand) => {
    if (brand === 'all' || !brand) {
      return selectPriceRange({ catalog: { products } } as RootState);
    }
    
    const filteredProducts = products.filter(p => p.brand === brand);
    if (filteredProducts.length === 0) return { min: 0, max: 100 };
    
    const prices = filteredProducts.map(p => p.price);
    return {
      min: Math.min(...prices),
      max: Math.max(...prices)
    };
  }
);

export const selectTotalPrice = createSelector(
  [selectSelectedProductsData],
  (selectedProducts) => selectedProducts.reduce((total, product) => total + product.price, 0)
);
