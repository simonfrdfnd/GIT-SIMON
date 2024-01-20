import requests
import pandas as pd

def get_trainings():
    # Fetch training data using PowerShell script
    uri = "https://api.holdsport.dk/v1/teams/238011/activities?date=2024-01-12"
    credentials = ("simon.froidefond", "21060511Aas")
    headers = {"Accept": "application/json"}

    response = requests.get(uri, headers=headers, auth=credentials)
    data = response.json()

    # Filter activities based on the event_type and status
    filtered_activities = [activity for activity in data if
                        activity.get('event_type') in ['Entrainement'] and activity.get('status') == 1]

    # Create a DataFrame from the filtered activities
    training_df = pd.DataFrame(filtered_activities, columns=['starttime', 'status', 'event_type'])

    # Convert 'starttime' to datetime and extract day
    training_df['starttime'] = pd.to_datetime(training_df['starttime'])
    training_df['Date'] = training_df['starttime'].dt.strftime('%Y-%m-%d')

    # Count the number of trainings per day
    trainings_per_day = training_df.groupby('Date').size().reset_index(name='Trainings')

    return trainings_per_day
    # Create a list containing the sum of trainings for each day in the input list
    # result = [trainings_per_day[trainings_per_day['day'] == day]['trainings'].sum() for day in days]
    # print(f"Rugby Trainings {result}")
    #return result

def get_games():
    # Fetch training data using PowerShell script
    uri = "https://api.holdsport.dk/v1/teams/238011/activities?date=2024-01-12"
    credentials = ("simon.froidefond", "21060511Aas")
    headers = {"Accept": "application/json"}

    response = requests.get(uri, headers=headers, auth=credentials)
    data = response.json()

    # Filter activities based on the event_type and status
    filtered_activities = [activity for activity in data if
                        activity.get('event_type') in ['Match'] and activity.get('status') == 1]

    # Create a DataFrame from the filtered activities
    game_df = pd.DataFrame(filtered_activities, columns=['starttime', 'status', 'event_type'])

    # Convert 'starttime' to datetime and extract day
    game_df['starttime'] = pd.to_datetime(game_df['starttime'])
    game_df['Date'] = game_df['starttime'].dt.strftime('%Y-%m-%d')

    # Count the number of trainings per day
    games_per_day = game_df.groupby('Date').size().reset_index(name='Games')

    return games_per_day    

def get_sport():
    entrainements = get_trainings()
    games = get_games()

    # Create new DataFrames with 'Date' and 'Sport' columns
    df_trainings = pd.DataFrame({'Date': entrainements['Date'], 'Sport': 'Training'})
    df_games = pd.DataFrame({'Date': games['Date'], 'Sport': 'Game'})

    # Merge DataFrames on 'Date' using outer join to combine Trainings and Games
    result_df = pd.merge(df_trainings, df_games, on='Date', how='outer')

    # Fill NaN values in 'Sport' column with the appropriate values
    result_df['Sport'] = result_df['Sport_x'].fillna(result_df['Sport_y'])

    # Drop unnecessary columns
    result_df = result_df.drop(['Sport_x', 'Sport_y'], axis=1)

    # Sort the DataFrame by 'Date'
    result_df = result_df.sort_values(by='Date').reset_index(drop=True)

    return result_df

# Run the app
if __name__ == '__main__':
    print(get_sport())
