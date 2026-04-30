function hue(name: string): number {
  let hash = 0;
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash);
  }
  return Math.abs(hash) % 360;
}

export function getCategoryBg(name: string): string {
  return `hsl(${hue(name)}, 55%, 88%)`;
}

export function getCategoryTextColor(name: string): string {
  return `hsl(${hue(name)}, 45%, 30%)`;
}
