import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-skeleton',
  standalone: true,
  template: `
    <div
      class="skeleton"
      [style.width]="width()"
      [style.height]="height()"
      [style.border-radius]="radius()"
      aria-hidden="true"
    ></div>
  `,
  styles: [`
    .skeleton {
      background: linear-gradient(
        90deg,
        rgba(255,255,255,0.04) 25%,
        rgba(255,255,255,0.08) 50%,
        rgba(255,255,255,0.04) 75%
      );
      background-size: 200% 100%;
      animation: shimmer 1.6s ease-in-out infinite;
    }

    @keyframes shimmer {
      from { background-position: 200% 0; }
      to   { background-position: -200% 0; }
    }

    @media (prefers-reduced-motion: reduce) {
      .skeleton { animation: none; background: rgba(255,255,255,0.06); }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkeletonComponent {
  readonly width  = input<string>('100%');
  readonly height = input<string>('16px');
  readonly radius = input<string>('6px');
}
