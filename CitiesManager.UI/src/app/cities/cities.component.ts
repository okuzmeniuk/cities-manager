import { Component } from '@angular/core';
import { City } from '../models/city';
import { CitiesService } from '../services/cities.service';
import {
  FormArray,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DisableControlDirective } from '../directives/disable-control.directive';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-cities',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, DisableControlDirective],
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.css',
})
export class CitiesComponent {
  cities: City[] = [];
  postCityForm: FormGroup;
  isPostCityFormSubmitted = false;

  putCityForm: FormGroup;
  editCityID: string | null = null;

  constructor(
    private citiesService: CitiesService,
    private accountService: AccountService
  ) {
    this.postCityForm = new FormGroup({
      cityName: new FormControl(null, [Validators.required]),
    });

    this.putCityForm = new FormGroup({
      cities: new FormArray([]),
    });
  }

  get putCityFormArray(): FormArray {
    return this.putCityForm.get('cities') as FormArray;
  }

  loadCities() {
    this.citiesService.getCities().subscribe({
      next: (response: City[]) => {
        this.cities = response;
        this.cities.forEach((city) => {
          this.putCityFormArray.push(
            new FormGroup({
              cityID: new FormControl(city.cityID, [Validators.required]),
              cityName: new FormControl(
                { value: city.cityName, disabled: true },
                [Validators.required]
              ),
            })
          );
        });
      },
      error: console.log,
    });
  }

  ngOnInit() {
    this.loadCities();
  }

  get postCity_CityNameControl() {
    return this.postCityForm.controls['cityName'];
  }

  postCitySubmitted() {
    this.isPostCityFormSubmitted = true;
    this.citiesService.postCity(this.postCityForm.value).subscribe({
      next: (response: City) => {
        console.log(response);

        this.putCityFormArray.push(
          new FormGroup({
            cityID: new FormControl(response.cityID, [Validators.required]),
            cityName: new FormControl(
              { value: response.cityName, disabled: true },
              [Validators.required]
            ),
          })
        );
        this.cities.push(new City(response.cityID, response.cityName));

        this.isPostCityFormSubmitted = false;
        this.postCityForm.reset();
      },
      error: console.log,
    });
  }

  editClicked(city: City): void {
    this.editCityID = city.cityID;
  }

  updateClicked(index: number): void {
    this.citiesService
      .putCity(this.putCityFormArray.controls[index].value)
      .subscribe({
        next: (response: string) => {
          this.editCityID = null;
          this.putCityFormArray.controls[index].reset(
            this.putCityFormArray.controls[index].value
          );
        },

        error: (err) => {
          console.log(err);
        },
      });
  }

  deleteClicked(city: City, i: number): void {
    if (confirm(`Are you sure to delete the city: ${city.cityName}`)) {
      this.citiesService.deleteCity(city.cityID).subscribe({
        next: (response: string) => {
          this.putCityFormArray.removeAt(i);
          this.cities.splice(i, 1);
        },
        error: (err) => {
          console.log(err);
        },
      });
    }
  }

  refreshClicked(): void {
    this.accountService.postGenerateNewToken().subscribe({
      next: (response: any) => {
        localStorage['token'] = response.token;
        localStorage['refreshToken'] = response.refreshToken;

        this.loadCities();
      },
      error: (err: any) => {
        console.log(err);
      },
    });
  }
}
