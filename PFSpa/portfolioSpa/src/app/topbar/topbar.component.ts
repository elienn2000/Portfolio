import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent {
  constructor(private translate: TranslateService) {
    // Aggiungo le lingue disponibili
    translate.addLangs(['it', 'en']);
    translate.setFallbackLang('it');
    translate.use('it'); // lingua di default
  }

  // Cambio lingua quando clicco sulla bandiera
  switchLang(lang: string) {
    this.translate.use(lang);
  }
}
