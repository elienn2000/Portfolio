import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { TranslateModule } from '@ngx-translate/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSelectModule } from '@angular/material/select';
import {MatFormFieldModule} from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [MatButtonModule, CommonModule, TranslateModule, MatToolbarModule, MatSelectModule, MatFormFieldModule],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent {

  selLang = 'en';
  selectedMenu = 'home';

  constructor(private translate: TranslateService) {
    // Aggiungo le lingue disponibili
    translate.addLangs(['it', 'en']);
    translate.setFallbackLang('it');
    this.switchLang();
  }

  // Cambio lingua quando clicco sulla bandiera
  switchLang() {
    this.translate.use(this.selLang);
  }
}
