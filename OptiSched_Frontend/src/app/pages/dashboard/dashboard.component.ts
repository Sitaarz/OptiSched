import {Component, OnInit} from '@angular/core';
import {UserService} from '../../shared/services/user.service';
import {CalendarWrapperComponent} from './calendar-wrapper/calendar-wrapper.component';
import {EventDialogComponent} from './event-dialog/event-dialog.component';
import {AvailabilityDialogComponent} from './availability-dialog/availability-dialog.component';

@Component({
  selector: 'app-dashboard',
  imports: [
    CalendarWrapperComponent,
    EventDialogComponent,
    AvailabilityDialogComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  protected user = ''

  constructor(private userService: UserService) {
  }

  ngOnInit(): void {
    this.userService.getUserProfile().subscribe(
      {
        next: (profile: string) => {
          this.user = profile;
        },
      })
  }

}
