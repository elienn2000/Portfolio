import { ApplicationConfig } from '@angular/core';
import { provideRouter, withEnabledBlockingInitialNavigation } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { routes } from './app.routes';

// ngx-translate
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader, TRANSLATE_HTTP_LOADER_CONFIG } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withEnabledBlockingInitialNavigation()), // 🔹 utile anche con prerender
    provideHttpClient(),

    // Configurazione loader: dice dove sono i file JSON
    {
      provide: TRANSLATE_HTTP_LOADER_CONFIG,
      useValue: {
        prefix: 'assets/i18n/', // 👈 senza ./ davanti
        suffix: '.json'
      }
    },

    // Provider di ngx-translate
    ...TranslateModule.forRoot({
      defaultLanguage: 'it',
      loader: {
        provide: TranslateLoader,
        useClass: TranslateHttpLoader,
        deps: [HttpClient]
      }
    }).providers!
  ]
};
