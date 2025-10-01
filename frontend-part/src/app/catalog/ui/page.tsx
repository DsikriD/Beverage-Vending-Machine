"use client";

import React, { useEffect, useCallback } from "react";
import { useRouter } from "next/navigation";
import { Text } from "@/shared/ui/Text";
import { HStack } from "@/shared/ui/HStack";
import { VStack } from "@/shared/ui/VStack";
import { Button } from "@/shared/ui/Button";
import { Select } from "@/shared/ui/Select";
import { RangeSlider } from "@/shared/ui/RangeSlider";
import { ProductCard } from "@/shared/ui/ProductCard";
import { useAppDispatch, useAppSelector } from "@/store/hooks";
import {
  selectProducts,
  selectFilteredProducts,
  selectBrands,
  selectFilters,
  selectLoading,
  selectError,
  selectPriceRange,
  selectImportLoading,
  selectImportError,
} from "../model/selector";
import {
  fetchProducts,
  fetchBrands,
  setFilters,
  importProducts,
} from "../model/productsSlice";
import { addToCart } from "../../cart/model/cartSlice";
import styles from "./page.module.scss";

const CatalogPage = () => {
  const dispatch = useAppDispatch();
  const router = useRouter();

  // Селекторы
  const products = useAppSelector(selectFilteredProducts);
  const brands = useAppSelector(selectBrands);
  const filters = useAppSelector(selectFilters);
  const loading = useAppSelector(selectLoading);
  const error = useAppSelector(selectError);
  const priceRange = useAppSelector(selectPriceRange);
  const importLoading = useAppSelector(selectImportLoading);
  const importError = useAppSelector(selectImportError);

  // Селекторы корзины
  const cartItems = useAppSelector((state) => state.cart.items);
  const selectedCount = cartItems.reduce(
    (total, item) => total + item.quantity,
    0
  );

  // Загружаем данные при монтировании компонента
  useEffect(() => {
    dispatch(fetchProducts({}));
    dispatch(fetchBrands());
  }, [dispatch]);

  // Обработчики
  const handleBrandChange = useCallback((brand: string) => {
    const newBrand = brand === "all" ? undefined : brand;
    dispatch(setFilters({ brand: newBrand }));
    // Сразу применяем фильтр по бренду
    dispatch(fetchProducts({ brand: newBrand }));
  }, [dispatch]);

  const handlePriceChange = useCallback((price: number) => {
    // Только обновляем состояние для клиентской фильтрации
    dispatch(setFilters({ maxPrice: price }));
  }, [dispatch]);

  const handleProceedToCart = () => {
    if (selectedCount > 0) {
      router.push("/cart");
    }
  };

  const handleProductSelect = (id: string) => {
    const product = allProducts.find((p) => p.id.toString() === id);
    if (product && product.isAvailable) {
      // Добавляем товар в корзину
      dispatch(
        addToCart({
          id: product.id.toString(),
          name: product.name,
          brand: product.brand,
          price: product.price,
          imageUrl: product.imageUrl || null,
          maxQuantity: product.stockQuantity,
        })
      );
    }
  };

  const handleImport = () => {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = ".xlsx,.xls,.csv";
    input.onchange = (e) => {
      const file = (e.target as HTMLInputElement).files?.[0];
      if (file) {
        dispatch(importProducts(file)).then(() => {
          // После успешного импорта обновляем данные
          dispatch(fetchProducts({}));
          dispatch(fetchBrands());
        });
      }
    };
    input.click();
  };

  const allProducts = useAppSelector(selectProducts);

  const getProductStatus = (id: string) => {
    const product = allProducts.find((p) => p.id.toString() === id);
    if (!product?.isAvailable) return "outOfStock";

    const cartItem = cartItems.find((item) => item.id === id);
    if (cartItem) return "selected";

    return "available";
  };

  // Преобразуем бренды для Select компонента
  const brandOptions = [
    { value: "all", label: "Все бренды" },
    ...brands.map((brand) => ({ value: brand, label: brand })),
  ];

  // Состояние загрузки
  if (loading && products.length === 0) {
    return (
      <div className={styles.page}>
        <VStack gap="lg" align="center">
          <Text size="2xl">Загрузка продуктов...</Text>
        </VStack>
      </div>
    );
  }

  // Состояние ошибки
  if (error) {
    return (
      <div className={styles.page}>
        <VStack gap="lg" align="center">
          <Text size="2xl" color="error">
            Ошибка: {error}
          </Text>
          <Button onClick={() => dispatch(fetchProducts({}))}>
            Попробовать снова
          </Button>
        </VStack>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      <VStack gap="lg" align="stretch">
        {/* Header */}
        <HStack justify="between" align="center">
          <Text size="4xl" color="primary" weight="bold">
            Газированные напитки
          </Text>
          <Button
            variant="outline"
            size="md"
            onClick={handleImport}
            disabled={importLoading}
          >
            {importLoading ? "Импорт..." : "Импорт"}
          </Button>
        </HStack>

        {/* Filters */}
        <HStack justify="between" gap="lg" align="end">
          <HStack gap="lg">
            <Select
              label="Бренд"
              options={brandOptions}
              placeholder="Выберите бренд"
              value={filters.brand || "all"}
              onChange={(e) => handleBrandChange(e.target.value)}
            />
            <RangeSlider
              label="Стоимость"
              min={priceRange.min}
              max={priceRange.max}
              value={filters.maxPrice || priceRange.max}
              onChange={handlePriceChange}
            />
          </HStack>
          <Button
            variant="secondary"
            size="md"
            disabled={selectedCount === 0}
            onClick={handleProceedToCart}
          >
            Выбрано: {selectedCount}
          </Button>
        </HStack>

        {/* Products Grid */}
        <div className={styles.grid}>
          {products.map((product) => (
            <ProductCard
              key={product.id}
              id={product.id.toString()}
              name={product.name}
              price={product.price}
              image={product.imageUrl || "/images/default.jpg"}
              status={getProductStatus(product.id.toString())}
              onSelect={handleProductSelect}
            />
          ))}
        </div>

        {/* Import error */}
        {importError && (
          <VStack gap="md" align="center">
            <Text size="lg" color="error">
              Ошибка импорта: {importError}
            </Text>
          </VStack>
        )}

        {/* Empty state */}
        {products.length === 0 && !loading && (
          <VStack gap="md" align="center">
            <Text size="xl">Продукты не найдены</Text>
            <Text>Попробуйте изменить фильтры</Text>
          </VStack>
        )}
      </VStack>
    </div>
  );
};

export default CatalogPage;
