import {
  ChangeDetectionStrategy,
  Component,
  signal,
} from '@angular/core';
import { NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ObserveVisibilityDirective } from '../../../shared/directives/observe-visibility.directive';
import { PersonalInfo, StatCard } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [NgClass, RouterLink, ObserveVisibilityDirective],
  templateUrl: './about.component.html',
  styleUrl: './about.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AboutComponent {

  // ── Visibility signals (driven by IntersectionObserver directive) ─────
  readonly leftVisible  = signal(false);
  readonly rightVisible = signal(false);

  // ── Bio paragraphs ─────────────────────────────────────────────────────
  readonly bioParagraphs: string[] = [
    `I'm a <strong>Full-Stack AI Engineer</strong> with a passion for building
     enterprise-grade applications that go far beyond CRUD. I design systems where
     <strong>.NET backends</strong> and <strong>Angular frontends</strong> are
     orchestrated by intelligent agents that think, reason, and act autonomously.`,

    `My focus sits at the intersection of <strong>Agentic AI workflows</strong>,
     <strong>RAG pipelines</strong>, and <strong>Model Context Protocol (MCP)</strong> —
     turning complex business problems into self-healing, auto-scaling intelligent
     platforms backed by solid <strong>DevOps foundations</strong>.`,

    `When I'm not architecting AI systems, I'm contributing to open-source tooling,
     exploring the latest in <strong>vector databases</strong>, and obsessing over
     clean code that future me will actually appreciate.`,
  ];

  // ── Personal info grid ─────────────────────────────────────────────────
  readonly personalInfo: PersonalInfo[] = [
    { label: 'Location',     value: 'Aligarh, India' },
    { label: 'Availability', value: 'Open to work',       highlight: 'green' },
    { label: 'Education',    value: 'B.Tech. Computer Science Engineering, 2024' },
    { label: 'Experience',   value: '1.5 years' },
    {
      label: 'GitHub',
      value: 'github.com/predator1604',
      isLink: true,
      href:  'https://github.com/predator1604',
      highlight: 'accent',
    },
    {
      label: 'LinkedIn',
      value: 'linkedin.com/in/prashant1604',
      isLink: true,
      href:  'https://www.linkedin.com/in/prashant1604',
      highlight: 'accent',
    },
  ];

  // ── Stat cards ─────────────────────────────────────────────────────────
  readonly statCards: StatCard[] = [
    { value: '3',  suffix: '+', description: 'AI-native projects shipped'  },
    { value: '10', suffix: '+', description: 'Technologies mastered'       },
    { value: '∞',  suffix: '',  description: 'Pipelines automated'         },
    { value: '01', suffix: '',  description: 'Philosophy: ship clean code' },
  ];

  // ── Code snippet lines ─────────────────────────────────────────────────
  readonly codeLines: { raw: string; type: string }[] = [
    { raw: '<span class="c-purple">const</span> <span class="c-blue">engineer</span> <span class="c-white">= {</span>', type: 'code' },
    { raw: '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c-green">name</span><span class="c-white">:</span> <span class="c-amber">\'Prashant Kumar Singh\'</span><span class="c-white">,</span>', type: 'code' },
    { raw: '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c-green">role</span><span class="c-white">:</span> <span class="c-amber">\'Full-Stack AI Engineer\'</span><span class="c-white">,</span>', type: 'code' },
    { raw: '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c-green">stack</span><span class="c-white">: [</span><span class="c-amber">\'.NET\'</span><span class="c-white">,</span> <span class="c-amber">\'Angular\'</span><span class="c-white">,</span> <span class="c-amber">\'MongoDB\'</span><span class="c-white">],</span>', type: 'code' },
    { raw: '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c-green">ai</span><span class="c-white">: [</span><span class="c-amber">\'Agentic\'</span><span class="c-white">,</span> <span class="c-amber">\'RAG\'</span><span class="c-white">,</span> <span class="c-amber">\'MCP\'</span><span class="c-white">],</span>', type: 'code' },
    { raw: '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c-green">devops</span><span class="c-white">: [</span><span class="c-amber">\'Docker\'</span><span class="c-white">,</span> <span class="c-amber">\'K8s\'</span><span class="c-white">,</span> <span class="c-amber">\'Helm\'</span><span class="c-white">],</span>', type: 'code' },
    { raw: '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c-green">available</span><span class="c-white">:</span> <span class="c-purple">true</span>', type: 'code' },
    { raw: '<span class="c-white">};</span>', type: 'code' },
    { raw: '', type: 'blank' },
    { raw: '<span class="c-muted">// always building something new 🚀</span>', type: 'comment' },
  ];
}
