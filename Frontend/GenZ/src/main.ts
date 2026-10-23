import { platformBrowser } from '@angular/platform-browser';
import { AppModuleModule } from '../src/app-module/app-module-module';

platformBrowser()
  .bootstrapModule(AppModuleModule)
  .catch((err: unknown) => console.error(err));