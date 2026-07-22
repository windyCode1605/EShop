import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { MessageService } from 'primeng/api';
import { CookieService } from 'ngx-cookie-service';

// Add the Firebase imports here:
import { provideFirebaseApp, initializeApp } from '@angular/fire/app';
import { provideMessaging, getMessaging } from '@angular/fire/messaging';
import { environment } from './my-lib/shared/enviroments/enviroment';
import { provideStorage, getStorage } from '@angular/fire/storage';


import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { apiUrlInterceptor } from './core/interceptors/api-url.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([apiUrlInterceptor, authInterceptor]),
      withInterceptorsFromDi()
    ),
    MessageService,
    CookieService,

    // Firebase Providers
    provideFirebaseApp(() => initializeApp(environment.firebase)),
    provideStorage(() => getStorage()),
    provideMessaging(() => getMessaging())
  ]
};
