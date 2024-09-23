import { Component } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { DisableControlDirective } from './directives/disable-control.directive';
import { AccountService } from './services/account.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  constructor(public accountService: AccountService, private router: Router) {}

  onLogOutClicked() {
    this.accountService.getLogout().subscribe({
      next: (response: string) => {
        this.accountService.currentUserName = null;
        localStorage.removeItem('token');
        this.router.navigate(['/login']);
      },

      error: (err) => {
        console.log(err);
      },
    });
  }
}
