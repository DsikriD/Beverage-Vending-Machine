import React from 'react';
import styles from './RangeSlider.module.scss';

interface RangeSliderProps {
  label?: string;
  min: number;
  max: number;
  value: number;
  onChange: (value: number) => void;
  step?: number;
  className?: string;
}

export const RangeSlider: React.FC<RangeSliderProps> = ({
  label,
  min,
  max,
  value,
  onChange,
  step = 1,
  className
}) => {
  const classes = [
    styles.container,
    className
  ].filter(Boolean).join(' ');

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = Number(e.target.value);
    // Валидируем значение
    if (newValue >= min && newValue <= max) {
      onChange(newValue);
    }
  };

  // Валидируем текущее значение
  const validValue = Math.max(min, Math.min(max, value));

  return (
    <div className={classes}>
      {label && <label className={styles.label}>{label}</label>}
      <div className={styles.sliderContainer}>
        <input
          type="range"
          min={min}
          max={max}
          value={validValue}
          step={step}
          onChange={handleChange}
          className={styles.slider}
        />
        <div className={styles.labels}>
          <span className={styles.minLabel}>{min} руб.</span>
          <span className={styles.maxLabel}>{max} руб.</span>
        </div>
      </div>
    </div>
  );
};
