import {
  ChangeDetectionStrategy,
  Component,
  PLATFORM_ID,
  computed,
  inject,
  signal,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { RouterLink } from '@angular/router';
import {
  FooterNavColumn,
  FooterNavLink,
  FooterSocialLink,
  FooterTechChip,
} from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FooterComponent {
  private readonly platformId = inject(PLATFORM_ID);

  // ── Current year — auto-updates at build time ──────────────────────
  readonly currentYear = computed(() => new Date().getFullYear());

  // ── Scroll-to-top visibility ───────────────────────────────────────
  readonly showScrollTop = signal(false);

  // ── Navigation columns ─────────────────────────────────────────────
  readonly navColumns: FooterNavColumn[] = [
    {
      title: 'Navigation',
      links: [
        { label: 'Home',     fragment: 'hero'     },
        { label: 'About',    fragment: 'about'    },
        { label: 'Skills',   fragment: 'skills'   },
        { label: 'Projects', fragment: 'projects' },
        { label: 'Contact',  fragment: 'contact'  },
      ],
    },
    {
      title: 'Projects',
      links: [
        { label: 'NexusAgent',    href: 'https://github.com/predator1604/nexusagent'    },
        { label: 'CognitiveFlow', href: 'https://github.com/predator1604/cognitiveflow' },
        { label: 'OpsMind',       href: 'https://github.com/predator1604/opsmind'       },
        { label: 'View all on GitHub →', href: 'https://github.com/predator1604', muted: true },
      ],
    },
  ];

  // ── Core tech chips ────────────────────────────────────────────────
  readonly techChips: FooterTechChip[] = [
    { label: '.NET'     },
    { label: 'Angular'  },
    { label: 'MongoDB'  },
    { label: 'MCP'      },
    { label: 'RAG'      },
    { label: 'Docker'   },
    { label: 'K8s'      },
    { label: 'Helm'     },
    { label: 'GenAI'    },
    { label: 'DevOps'   },
  ];

  // ── Social links ───────────────────────────────────────────────────
  readonly socialLinks: FooterSocialLink[] = [
    {
      id: 'github',
      label: 'GitHub',
      href: 'https://github.com/predator1604',
      iconPath: `<path d="M8 1C4.13 1 1 4.13 1 8c0 3.09 2.01 5.71 4.79 6.64.35.06.48-.15.48-.34v-1.19c-1.95.42-2.36-.94-2.36-.94-.32-.81-.78-1.03-.78-1.03-.64-.43.05-.42.05-.42.7.05 1.07.72 1.07.72.62 1.07 1.63.76 2.03.58.06-.45.24-.76.44-.94-1.56-.18-3.2-.78-3.2-3.46 0-.76.27-1.39.72-1.88-.07-.18-.31-.89.07-1.85 0 0 .59-.19 1.92.72A6.67 6.67 0 018 4.79c.59 0 1.19.08 1.75.23 1.33-.9 1.92-.72 1.92-.72.38.96.14 1.67.07 1.85.45.49.72 1.12.72 1.88 0 2.69-1.64 3.28-3.2 3.46.25.22.48.65.48 1.31v1.94c0 .19.13.4.48.34C12.99 13.71 15 11.09 15 8c0-3.87-3.13-7-7-7z" fill="currentColor"/>`,
    },
    {
      id: 'linkedin',
      label: 'LinkedIn',
      href: 'https://www.linkedin.com/in/prashant1604',
      iconPath: `<path d="M13.5 1h-11A1.5 1.5 0 001 2.5v11A1.5 1.5 0 002.5 15h11a1.5 1.5 0 001.5-1.5v-11A1.5 1.5 0 0013.5 1zM5 6.5v6M5 4.25a.75.75 0 110-1.5.75.75 0 010 1.5zM8 6.5v6M8 9c0-1.5 1-2.5 2.25-2.5S12.5 7.5 12.5 9v3.5" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/>`,
    },
    {
      id: 'email',
      label: 'Email',
      href: 'mailto:rajpootprashant1604@gmail.com',
      iconPath: `<path d="M2 4l6 4 6-4M2 4v8a1 1 0 001 1h10a1 1 0 001-1V4M2 4a1 1 0 011-1h10a1 1 0 011 1" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/>`,
    },
  ];

  // ── Scroll to top ──────────────────────────────────────────────────
  scrollToTop(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  // ── TrackBy helpers ────────────────────────────────────────────────
  trackColumn(_: number, col: FooterNavColumn): string  { return col.title; }
  trackLink(_: number, link: FooterNavLink): string     { return link.label; }
  trackSocial(_: number, s: FooterSocialLink): string   { return s.id; }
  trackChip(_: number, chip: FooterTechChip): string    { return chip.label; }
}
