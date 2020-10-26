import { OverlayModule } from '@angular/cdk/overlay';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatAutocompleteModule,
  MatButtonModule,
  MatCheckboxModule,
  MatDialogModule,
  MatFormFieldModule,
  MatGridListModule,
  MatIconModule,
  MatInputModule,
  MatNativeDateModule,
  MatPaginatorModule,
  MatProgressSpinnerModule,
  MatSelectModule,
  MatSnackBarModule,
  MatSortModule,
  MatSpinner,
  MatTableModule,
  MatToolbarModule,
} from '@angular/material';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';

import { DataService } from './../services/data.service';
import { AdminContentComponent } from './admin-content/admin-content.component';
import { VotersOptionEditorComponent } from './admin-content/voters-option-editor/voters-option-editor.component';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { ImportPollingStationsComponent } from './import-polling-stations/import-polling-stations.component';
import {
  ImportedPollingStationsTableComponent,
} from './import-polling-stations/imported-polling-stations-table/imported-polling-stations-table.component';
import { PollingStationEditorComponent } from './imported-polling-station-editor/polling-station-editor.component';
import { LoginComponent } from './login/login.component';
import { AdminEffects } from './state/admin-effects';
import { adminStateReducer } from './state/admin-reducers';
import { AuthEffects } from './state/auth';

@NgModule({
  imports: [
    CommonModule,
    AdminRoutingModule,
    FormsModule,
    MatAutocompleteModule,
    MatInputModule,
    MatPaginatorModule,
    MatTableModule,
    MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    MatGridListModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCheckboxModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatSortModule,
    ReactiveFormsModule,
    AngularEditorModule,
    MatProgressSpinnerModule,
    OverlayModule,
    StoreModule.forFeature('admin', adminStateReducer),
    EffectsModule.forFeature([AdminEffects, AuthEffects]),
  ],
  declarations: [
    LoginComponent,
    AdminComponent,
    AdminContentComponent,
    VotersOptionEditorComponent,
    ImportPollingStationsComponent,
    ImportedPollingStationsTableComponent,
    PollingStationEditorComponent
  ],
  entryComponents: [
    VotersOptionEditorComponent,
    PollingStationEditorComponent,
    MatSpinner
  ],
  providers: [
    DataService
  ],
})
export class AdminModule { }
