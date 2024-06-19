using EgoTournament.Common;
using EgoTournament.Models;

namespace EgoTournament.Adapters.Converters
{
    public class ParticipantTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ChampionTemplate { get; set; }
        public DataTemplate SecondTemplate { get; set; }
        public DataTemplate ThirdTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var participant = item as ParticipantResultDto;
            return participant.Position switch
            {
                Globals.CHAMPION_POSITION => ChampionTemplate,
                Globals.SECOND_POSITION => SecondTemplate,
                Globals.THIRD_POSITION => ThirdTemplate,
                _ => DefaultTemplate,
            };
        }
    }
}