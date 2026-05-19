# Contributing

## Project Standards

### Shared Styling
- Use `wwwroot/css/site.css` as the global design system for shared layout, typography, spacing, buttons, cards, forms, alerts, and reusable page sections.
- Page-specific styles should be added in dedicated CSS files and loaded through the view `Styles` section.
- Page-specific CSS should extend or override the global stylesheet rather than redefining the full visual system.
- Keep colors, spacing, border radius, shadows, and interactive states visually consistent across pages.
- Prefer reusable CSS classes and variables over one-off inline styles.

## Frontend Guidelines
- Match the existing visual style used across event, FAQ, announcements, and home pages.
- Use responsive layouts that adapt cleanly to tablet and mobile widths.
- Avoid duplicating shared component styling across multiple page CSS files.
- Keep page markup simple and let shared styling handle the common look and feel.

## Workflow
- Add shared styles to `site.css` first when they can be reused.
- Only add page-level overrides when the page needs unique presentation.
- Test new styles on desktop and mobile breakpoints before submitting changes.