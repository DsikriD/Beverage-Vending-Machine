import React from 'react';
import styles from './Select.module.scss';

interface SelectOption {
  value: string;
  label: string;
}

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  options: SelectOption[];
  placeholder?: string;
  fullWidth?: boolean;
  className?: string;
}

export const Select: React.FC<SelectProps> = ({
  label,
  options,
  placeholder,
  fullWidth = false,
  className,
  ...props
}) => {
  const classes = [
    styles.select,
    fullWidth && styles.fullWidth,
    className
  ].filter(Boolean).join(' ');

  return (
    <div className={styles.container}>
      {label && <label className={styles.label}>{label}</label>}
      <select className={classes} {...props}>
        {placeholder && (
          <option value="" disabled>
            {placeholder}
          </option>
        )}
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </div>
  );
};
