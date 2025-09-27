import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { config } from './app/app.config.server';


export default async function bootstrap(context: any) {
    return await bootstrapApplication(App, config, context);
}
