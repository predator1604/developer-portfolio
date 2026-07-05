import { Injectable, PLATFORM_ID, OnDestroy, inject, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type SectionId = 'hero' | 'about' | 'skills' | 'projects' | 'contact';

@Injectable({ providedIn: 'root' })
export class ScrollSpyService implements OnDestroy {
  private readonly platformId = inject(PLATFORM_ID);
  private observer!: IntersectionObserver;

  private readonly sectionIds: SectionId[] = ['hero', 'about', 'skills', 'projects', 'contact'];
  readonly activeSection = signal<SectionId>('hero');
  private readonly ratioMap = new Map<SectionId, number>();

  constructor() {
    if (!isPlatformBrowser(this.platformId)) return;
    setTimeout(() => this.init(), 0);
  }

  private init(): void {
    this.observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          const id = entry.target.id as SectionId;
          this.ratioMap.set(id, entry.intersectionRatio);
        }

        let topId: SectionId = 'hero';
        let topRatio = -1;

        for (const id of this.sectionIds) {
          const ratio = this.ratioMap.get(id) ?? 0;
          if (ratio > topRatio) {
            topRatio = ratio;
            topId = id;
          }
        }

        this.activeSection.set(topId);
      },
      {
        threshold: [0, 0.1, 0.2, 0.3, 0.4, 0.5],
        rootMargin: '-10% 0px -10% 0px',
      }
    );

    for (const id of this.sectionIds) {
      const el = document.getElementById(id);
      if (el) this.observer.observe(el);
    }
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}
