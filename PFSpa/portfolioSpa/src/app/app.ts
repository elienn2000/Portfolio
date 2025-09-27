import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TopbarComponent } from "./topbar/topbar.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TopbarComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'portfolioSpa';
}
