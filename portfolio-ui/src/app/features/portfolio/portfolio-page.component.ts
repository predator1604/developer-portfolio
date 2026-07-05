import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
} from '@angular/core';
import { HeroComponent }        from '../hero/components/hero.component';
import { AboutComponent }       from '../about/components/about.component';
import { SkillsComponent }      from '../skills/components/skills.component';
import { ProjectsComponent }    from '../projects/components/projects.component';
import { ContactComponent }     from '../contact/components/contact.component';
import { FooterComponent }      from '../footer/components/footer.component';
import { ScrollToTopComponent } from '../../shared/components/scroll-to-top/scroll-to-top.component';
import { ToastComponent }       from '../../shared/components/toast/toast.component';
import { ChatComponent }        from '../chat/components/chat.component';
import { SeoService }           from '../../core/services/seo.service';

/**
 * PortfolioPageComponent — root shell.
 * All sections wired. Live API data. Toast + AI Chat mounted globally.
 */
@Component({
  selector: 'app-portfolio-page',
  standalone: true,
  imports: [
    HeroComponent,
    AboutComponent,
    SkillsComponent,
    ProjectsComponent,
    ContactComponent,
    FooterComponent,
    ScrollToTopComponent,
    ToastComponent,
    ChatComponent,
  ],
  template: `
    <main>
      <app-hero />
      <app-about />
      <app-skills />
      <app-projects />
      <app-contact />
    </main>
    <app-footer />

    <!-- Global overlays -->
    <app-scroll-to-top />
    <app-toast />
    <app-chat />
  `,
  styles: [`
    main { display: block; background: #0a0a0f; }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PortfolioPageComponent implements OnInit {
  private readonly seo = inject(SeoService);

  ngOnInit(): void {
    this.seo.set({
      title:         'Prashant Kumar Singh — Full-Stack AI Engineer',
      description:   'Full-Stack AI Engineer specializing in enterprise .NET and Angular applications supercharged by Agentic AI, RAG pipelines, Model Context Protocol, and DevOps automation.',
      keywords:      '.NET, Angular, Agentic AI, RAG, MCP, MongoDB, Kubernetes, Docker, Full-Stack, AI Engineer',
      url:           'https://yourname.dev',
      image:         'https://yourname.dev/assets/og-image.png',
      twitterHandle: '@yourhandle',
    });
  }
}
