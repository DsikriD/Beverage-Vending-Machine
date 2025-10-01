import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { ProductsFilters, CatalogState } from '../types/product';
import { ProductsApi } from '../service/productsApi';
import { ImportApi } from '../service/importApi';

// Асинхронные thunk'и
export const fetchProducts = createAsyncThunk(
  'catalog/fetchProducts',
  async (filters: ProductsFilters = {}, { rejectWithValue }) => {
    try {
      // Проверяем, что мы на клиенте
      if (typeof window === 'undefined') {
        return [];
      }
      
      const response = await ProductsApi.getProducts(filters);
      return response;
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to fetch products');
    }
  }
);

export const fetchBrands = createAsyncThunk(
  'catalog/fetchBrands',
  async (_, { rejectWithValue }) => {
    try {
      // Проверяем, что мы на клиенте
      if (typeof window === 'undefined') {
        return [];
      }
      
      const response = await ProductsApi.getBrands();
      return response;
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to fetch brands');
    }
  }
);

export const importProducts = createAsyncThunk(
  'catalog/importProducts',
  async (file: File, { rejectWithValue }) => {
    try {
      // Проверяем, что мы на клиенте
      if (typeof window === 'undefined') {
        throw new Error('Import is only available on the client side');
      }
      
      const response = await ImportApi.importProducts(file);
      return response;
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to import products');
    }
  }
);

const initialState: CatalogState = {
  products: [],
  brands: [],
  selectedProducts: [],
  filters: {
    page: 1,
    pageSize: 20,
  },
  pagination: {
    currentPage: 1,
    totalPages: 1,
    totalCount: 0,
    hasNextPage: false,
    hasPreviousPage: false,
  },
  loading: false,
  error: null,
  importLoading: false,
  importError: null,
};

const catalogSlice = createSlice({
  name: 'catalog',
  initialState,
  reducers: {
    setFilters: (state, action: PayloadAction<Partial<ProductsFilters>>) => {
      state.filters = { ...state.filters, ...action.payload };
    },
    toggleProductSelection: (state, action: PayloadAction<string>) => {
      const productId = action.payload;
      const index = state.selectedProducts.indexOf(productId);
      
      if (index > -1) {
        state.selectedProducts.splice(index, 1);
      } else {
        state.selectedProducts.push(productId);
      }
    },
    clearSelectedProducts: (state) => {
      state.selectedProducts = [];
    },
    setSelectedProducts: (state, action: PayloadAction<string[]>) => {
      state.selectedProducts = action.payload;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    // fetchProducts
    builder
      .addCase(fetchProducts.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchProducts.fulfilled, (state, action) => {
        state.loading = false;
        state.products = action.payload;
      })
      .addCase(fetchProducts.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });

    // fetchBrands
    builder
      .addCase(fetchBrands.fulfilled, (state, action) => {
        state.brands = action.payload;
      })
      .addCase(fetchBrands.rejected, (state, action) => {
        state.error = action.payload as string;
      });

    // importProducts
    builder
      .addCase(importProducts.pending, (state) => {
        state.importLoading = true;
        state.importError = null;
      })
      .addCase(importProducts.fulfilled, (state) => {
        state.importLoading = false;
        state.importError = null;
        // После успешного импорта обновляем продукты и бренды
      })
      .addCase(importProducts.rejected, (state, action) => {
        state.importLoading = false;
        state.importError = action.payload as string;
      });
  },
});

export const {
  setFilters,
  toggleProductSelection,
  clearSelectedProducts,
  setSelectedProducts,
  clearError,
} = catalogSlice.actions;

export default catalogSlice.reducer;
