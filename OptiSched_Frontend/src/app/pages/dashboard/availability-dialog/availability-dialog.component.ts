import {Component} from '@angular/core';
import {ButtonComponent} from '@progress/kendo-angular-buttons';
import {DialogActionsComponent, DialogComponent} from '@progress/kendo-angular-dialog';
import {NgIf} from '@angular/common';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {EventServiceService} from '../../../shared/services/event-service.service';
import {CreateFormGroupArgs, KENDO_SCHEDULER, SchedulerEvent} from '@progress/kendo-angular-scheduler';
import {UserService} from '../../../shared/services/user.service';
import {AuthService} from '../../../shared/services/auth.service';

export interface Availability {
  Start: string,
  End: string,
}

@Component({
  selector: 'app-availability-dialog',
  imports: [
    ButtonComponent,
    DialogActionsComponent,
    DialogComponent,
    NgIf,
    ReactiveFormsModule,
    KENDO_SCHEDULER
  ],
  templateUrl: './availability-dialog.component.html',
  styleUrl: './availability-dialog.component.scss'
})
export class AvailabilityDialogComponent {
  protected dialogOpened = false;

  public selectedDate: Date = new Date();
  public formGroup: FormGroup = new FormGroup({});

  public events: SchedulerEvent[];

  constructor(
    private eventService: EventServiceService,
    private formBuilder: FormBuilder,
    private userService: UserService,
    private authService: AuthService
  ) {
    this.createFormGroup = this.createFormGroup.bind(this); // Keep this if needed for external usage
    this.events = [];

    // Subscribe to the dialog state changes
    this.eventService.getAddAvailabilityDialogSubscribtion().subscribe((value: boolean) => {
      this.dialogOpened = value;

      // Fetch user availabilities when the dialog is opened
      if (value) {
        this.userService.getUserAvabilities().subscribe({
          next: (res) => {
            console.log(res);
            this.events = res.map(function (event: SchedulerEvent) {
              return {
                ...event, // Spread the original properties
                start: new Date(event.start), // Convert start to Date object
                end: new Date(event.end) // Convert end to Date object
              };
            });
          },
          error: (err) => console.error('Error fetching user availabilities:', err)
        });
      }
    });
  }


  public createFormGroup(args: CreateFormGroupArgs): FormGroup {
    const dataItem = args.dataItem;

    this.formGroup = this.formBuilder.group(
      {
        id: args.isNew ? this.getNextId() : dataItem.id,
        start: [dataItem.start, Validators.required],
        end: [dataItem.end, Validators.required],
        title: "available"
      },
      {
        validator: this.startEndValidator,
      }
    );

    return this.formGroup;
  }

  public getNextId(): number {
    const len = this.events.length;

    return len === 0 ? 1 : this.events[this.events.length - 1].id + 1;
  }

  private startEndValidator = (fg: FormGroup) => {
    const start = fg.get("start")!.value;
    const end = fg.get("end")!.value;

    if (start !== null && end !== null && start.getTime() < end.getTime()) {
      return null;
    } else {
      return {range: "End date must be greater than Start date"};
    }
  };

  protected closeDialog(): void {
    this.dialogOpened = false;
  }

  protected onSubmit(): void {
    this.closeDialog();
    const avabilityToSave: Availability[] = this.events.map(term => {
      return {
        Start: term.start.toISOString(),
        End: term.end.toISOString(),
      }
    })

    this.userService.postAvailability(avabilityToSave).subscribe(
      {
        next: res => {
          console.log(res)
        },
        error: err => {
          console.log(err)
        }

      }
    );

    console.log(avabilityToSave);

  }
}
