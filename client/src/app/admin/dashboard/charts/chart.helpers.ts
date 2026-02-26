/** Shared chart palette and helpers used across dashboard chart components. */
export const CHART_COLORS = [
  '#6366f1', // indigo
  '#8b5cf6', // violet
  '#06b6d4', // cyan
  '#10b981', // emerald
  '#f59e0b', // amber
  '#ef4444', // red
  '#ec4899', // pink
  '#14b8a6', // teal
  '#f97316', // orange
  '#84cc16', // lime
];

export const CHART_FONT_FAMILY = 'Inter, sans-serif';

export function baseChartOptions(overrides: Record<string, any> = {}): Record<string, any> {
  return {
    chart: {
      fontFamily: CHART_FONT_FAMILY,
      toolbar: { show: false },
      background: 'transparent',
      ...overrides['chart'],
    },
    colors: CHART_COLORS,
    grid: {
      borderColor: '#e2e8f0',
      strokeDashArray: 4,
      ...overrides['grid'],
    },
    tooltip: {
      theme: 'light',
      style: { fontSize: '12px', fontFamily: CHART_FONT_FAMILY },
      ...overrides['tooltip'],
    },
    dataLabels: { enabled: false, ...overrides['dataLabels'] },
    legend: {
      fontFamily: CHART_FONT_FAMILY,
      fontSize: '12px',
      labels: { colors: '#64748b' },
      ...overrides['legend'],
    },
  };
}

/** Format large numbers as abbreviated strings (e.g. 1.2M, 45K). */
export function abbreviateNumber(value: number): string {
  if (value >= 1_000_000) return (value / 1_000_000).toFixed(1) + 'M';
  if (value >= 1_000) return (value / 1_000).toFixed(1) + 'K';
  return value.toFixed(0);
}
