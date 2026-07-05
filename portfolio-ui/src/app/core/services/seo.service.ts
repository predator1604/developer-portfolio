import { Injectable, inject } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';

export interface SeoConfig {
  title: string;
  description: string;
  keywords?: string;
  url?: string;
  image?: string;
  twitterHandle?: string;
}

@Injectable({ providedIn: 'root' })
export class SeoService {
  private readonly title = inject(Title);
  private readonly meta = inject(Meta);

  set(config: SeoConfig): void {
    this.title.setTitle(config.title);
    this.updateTag('description', config.description);
    if (config.keywords) {
      this.updateTag('keywords', config.keywords);
    }

    this.updateProp('og:type', 'website');
    this.updateProp('og:title', config.title);
    this.updateProp('og:description', config.description);
    if (config.url) this.updateProp('og:url', config.url);
    if (config.image) this.updateProp('og:image', config.image);

    this.updateName('twitter:card', 'summary_large_image');
    this.updateName('twitter:title', config.title);
    this.updateName('twitter:description', config.description);
    if (config.image) this.updateName('twitter:image', config.image);
    if (config.twitterHandle) this.updateName('twitter:creator', config.twitterHandle);
  }

  private updateTag(name: string, content: string): void {
    this.meta.updateTag({ name, content });
  }

  private updateProp(property: string, content: string): void {
    this.meta.updateTag({ property, content });
  }

  private updateName(name: string, content: string): void {
    this.meta.updateTag({ name, content });
  }
}
