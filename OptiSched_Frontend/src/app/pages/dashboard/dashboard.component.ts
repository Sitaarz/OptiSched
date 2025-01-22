import {Component, OnInit} from '@angular/core';
import {UserService} from '../../shared/services/user.service';
import {CalendarWrapperComponent} from './calendar-wrapper/calendar-wrapper.component';
import {EventDialogComponent} from './event-dialog/event-dialog.component';
import {AvailabilityDialogComponent} from './availability-dialog/availability-dialog.component';
import {KENDO_BUTTONS} from '@progress/kendo-angular-buttons';

@Component({
  selector: 'app-dashboard',
  imports: [
    CalendarWrapperComponent,
    EventDialogComponent,
    AvailabilityDialogComponent,
    KENDO_BUTTONS
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  protected user = ''
  public selectedRoom = 1

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

  onClick(roomNumber: number) {
    this.selectedRoom = roomNumber;
  }

}
