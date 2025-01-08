import {Component, OnInit} from '@angular/core';
import {MatToolbar} from '@angular/material/toolbar';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {Router, RouterLink} from '@angular/router';
import {NgIf} from '@angular/common';
import {AuthService} from '../shared/services/auth.service';
import {EventServiceService} from '../shared/services/event-service.service';
import {UserService} from '../shared/services/user.service';


@Component({
  selector: 'app-navbar',
  imports: [
    MatToolbar,
    MatButton,
    RouterLink,
    NgIf,
    MatIcon
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  protected title = 'OptiSched_Frontend';
  protected isLoggedIn: boolean = false;

  constructor(private authService: AuthService, protected router: Router, private eventService: EventServiceService,
              private userService: UserService,) {
  }

  ngOnInit(): void {
    // Initialize `isLoggedIn` in the lifecycle hook to ensure proper timing
    this.isLoggedIn = this.authService.isLoggedIn();
  }

  protected logout(): void {
    this.authService.logout();
  }

  protected openAddMeetingDialog(): void {
    this.eventService.triggerOpenAddMeetingDialogButton();
  }

  protected openAddAvailabilityDialog(): void {
    this.eventService.triggerAddAvailabilityDialogButton()
  }

  protected startProgram(): void {
    this.userService.postStartProgram().subscribe({
      next: event => {
        console.log(event);
      }
    })
  }
}
