import {Component} from '@angular/core';
import {EventServiceService} from '../../../shared/services/event-service.service';
import {DialogActionsComponent, DialogComponent} from '@progress/kendo-angular-dialog';
import {NgIf} from '@angular/common';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {LabelComponent} from '@progress/kendo-angular-label';
import {NumericTextBoxComponent} from '@progress/kendo-angular-inputs';
import {ButtonComponent} from '@progress/kendo-angular-buttons';
import {AutoCompleteComponent} from '@progress/kendo-angular-dropdowns';
import {UserService} from '../../../shared/services/user.service';

export interface UserDto {
  id: string,
  name: string,
  surname: string,
  email: string,
}

export interface Meeting {
  GuestId: string,
  Duration: number
}

@Component({
  selector: 'app-event-dialog',
  imports: [
    DialogComponent,
    NgIf,
    LabelComponent,
    NumericTextBoxComponent,
    ReactiveFormsModule,
    ButtonComponent,
    DialogActionsComponent,
    AutoCompleteComponent
  ],
  templateUrl: './event-dialog.component.html',
  styleUrl: './event-dialog.component.scss'
})
export class EventDialogComponent {
  protected addMeetingDialogOpened = false;
  protected formGroup: FormGroup;
  protected autocompleteData: string[] = []
  protected userList: UserDto[] = [];

  constructor(private eventService: EventServiceService, private userService: UserService) {
    this.eventService.getOpenAddMeetingDialogSubscribtion().subscribe((shouldOpen) => this.addMeetingDialogOpened = shouldOpen);
    this.formGroup = new FormGroup({
      guest: new FormControl(null, [Validators.required]),
      duration: new FormControl<number>(1, [Validators.required, Validators.min(1), Validators.max(8)]),
    })

    userService.getAllUsers().subscribe({
      next: (result: UserDto[]) => {
        this.autocompleteData = result.map(user => `${user.name} ${user.surname}`)
        this.userList = result
      }
      ,
      error: (err) => console.error('Failed to fetch users:', err)
    });

  }

  protected closeDialog(): void {
    this.addMeetingDialogOpened = false;
  }

  protected onSubmit() {
    this.addMeetingDialogOpened = false;
    const guest = this.formGroup.controls['guest'].value.split(' ');
    const guestId = this.userList.filter(user => user.name == guest[0] && user.surname == guest[1])[0].id;

    const meetingToSend: Meeting = {
      GuestId: guestId,
      Duration: this.formGroup.controls['duration'].value
    }

    this.userService.postMeeting(meetingToSend).subscribe({
      next: () => {
      },
      error: (err) => {
        console.log(err)
      }
    })

  }
}
