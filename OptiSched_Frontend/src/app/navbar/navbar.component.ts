import {Component, OnInit} from '@angular/core';
import {MatToolbar} from '@angular/material/toolbar';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {Router, RouterLink} from '@angular/router';
import {NgIf} from '@angular/common';
import {AuthService} from '../shared/services/auth.service';
import {EventServiceService} from '../shared/services/event-service.service';
import {UserService} from '../shared/services/user.service';
import {KENDO_INDICATORS} from '@progress/kendo-angular-indicators';


@Component({
  selector: 'app-navbar',
  imports: [
    MatToolbar,
    MatButton,
    RouterLink,
    NgIf,
    MatIcon,
    KENDO_INDICATORS
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  protected title = 'OptiSched_Frontend';
  protected isLoggedIn: boolean = false;
  protected isProgramRunning: boolean = false;

  constructor(private authService: AuthService, protected router: Router, private eventService: EventServiceService,
              private userService: UserService,) {
  }

  ngOnInit(): void {
    // Initialize `isLoggedIn` in the lifecycle hook to ensure proper timing
    this.isLoggedIn = this.authService.isLoggedIn();
    this.eventService.getLogInEvent().subscribe(logIn => this.isLoggedIn = logIn);
  }

  protected logout(): void {
    this.authService.logout();
    this.isLoggedIn = false;
  }

  protected openAddMeetingDialog(): void {
    this.eventService.triggerOpenAddMeetingDialogButton();
  }

  protected openAddAvailabilityDialog(): void {
    this.eventService.triggerAddAvailabilityDialogButton()
  }

  protected startProgram(): void {
    this.isProgramRunning = true;
    this.userService.postStartProgram().subscribe({
      next: event => {
        this.isProgramRunning = false;
        this.eventService.triggerDownloadeScheduleSubscribtion();
      },
      error: err => {
        this.isProgramRunning = false;
        this.eventService.triggerDownloadeScheduleSubscribtion();
      }
    })
  }
}
