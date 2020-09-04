import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { MatInputModule, MatDialogModule, MatAutocompleteModule, MatPaginatorModule, MatTableModule, MatSelectModule, MatSnackBarModule, MatToolbarModule, MatGridListModule, MatFormFieldModule, MatCheckboxModule, MatNativeDateModule, MatButtonModule, MatIconModule, MatSortModule, MatProgressSpinnerModule, MatSpinner } from '@angular/material';
import { LoginComponent } from './login/login.component';
import { AdminComponent } from './admin.component';
import { AdminContentComponent } from './admin-content/admin-content.component';
import { VotersOptionEditorComponent } from './admin-content/voters-option-editor/voters-option-editor.component';
import { AdminRoutingModule } from './admin-routing.module';
import { DataService } from './../services/data.service';
import { ImportPollingStationsComponent } from './import-polling-stations/import-polling-stations.component';
import { ImportedPollingStationsTableComponent } from './import-polling-stations/imported-polling-stations-table/imported-polling-stations-table.component';
import { PollingStationEditorComponent } from './imported-polling-station-editor/polling-station-editor.component';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { AdminEffects } from './state/admin-effects';
import { adminStateReducer } from './state/admin-reducers';
import { AuthEffects } from './state/auth';
import { OverlayModule } from '@angular/cdk/overlay';


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
    OverlayModule ,
    StoreModule.forFeature('admin', adminStateReducer),
    EffectsModule.forFeature([ AdminEffects, AuthEffects ]),
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
