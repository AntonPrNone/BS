using LogicLibrary;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BS
{
    /// <summary>
    /// Логика взаимодействия для PersInfo.xaml
    /// </summary>
    public partial class PersInfo : Window
    {
        public PersInfo(BrawlersDocument brawlers)
        {
            InitializeComponent();

            GridDate.DataContext = brawlers;
            Distribution(brawlers);
        }

        private void Distribution(BrawlersDocument brawlers)
        {
            Block0.Visibility = Visibility.Hidden;
            BlockF.Visibility = Visibility.Hidden;
            Block1.Visibility = Visibility.Hidden;
            Block2.Visibility = Visibility.Hidden;
            Block3.Visibility = Visibility.Hidden;
            Block4.Visibility = Visibility.Hidden;

            if (brawlers.Stats.CompanionAttack == "" && brawlers.Stats.CompanionHealth == ""
                && brawlers.Stats.CompanionSpeed == "" && brawlers.Stats.CompanionDistance == "")
            {
                if (brawlers.Stats.SuperAttackValue != "" && brawlers.Stats.SuperAttackDistance != ""
                    && brawlers.Stats.SuperAttackAdditionalItemName != "" && brawlers.Stats.SuperAttackAdditionalItemValue != "")
                {
                    TextBlockName1.Text = "Урон";
                    TextBlockName2.Text = "Дальность";
                    TextBlockName3.Text = brawlers.Stats.SuperAttackAdditionalItemName;

                    TextBlockValue1.Text = brawlers.Stats.SuperAttackValue;
                    TextBlockValue2.Text = brawlers.Stats.SuperAttackDistance;
                    TextBlockValue3.Text = brawlers.Stats.SuperAttackAdditionalItemValue;
                }


                else if (brawlers.Stats.SuperAttackValue != "" && brawlers.Stats.SuperAttackDistance != ""
                    && brawlers.Stats.SuperAttackAdditionalItemName == "" && brawlers.Stats.SuperAttackAdditionalItemValue == "")
                {
                    TextBlockName1.Text = "Урон";
                    TextBlockName2.Text = "Дальность";

                    TextBlockValue1.Text = brawlers.Stats.SuperAttackValue;
                    TextBlockValue2.Text = brawlers.Stats.SuperAttackDistance;
                }

                else if (brawlers.Stats.SuperAttackValue != "" && brawlers.Stats.SuperAttackDistance == ""
                    && brawlers.Stats.SuperAttackAdditionalItemName != "" && brawlers.Stats.SuperAttackAdditionalItemValue != "")
                {
                    TextBlockName1.Text = "Урон";
                    TextBlockName2.Text = brawlers.Stats.SuperAttackAdditionalItemName;

                    TextBlockValue1.Text = brawlers.Stats.SuperAttackValue;
                    TextBlockValue2.Text = brawlers.Stats.SuperAttackAdditionalItemValue;
                }

                else if (brawlers.Stats.SuperAttackValue == "" && brawlers.Stats.SuperAttackDistance != ""
                    && brawlers.Stats.SuperAttackAdditionalItemName != "" && brawlers.Stats.SuperAttackAdditionalItemValue != "")
                {
                    TextBlockName1.Text = "Дальность";
                    TextBlockName2.Text = brawlers.Stats.SuperAttackAdditionalItemName;

                    TextBlockValue1.Text = brawlers.Stats.SuperAttackDistance;
                    TextBlockValue2.Text = brawlers.Stats.SuperAttackAdditionalItemValue;
                }


                else if (brawlers.Stats.SuperAttackValue != "" && brawlers.Stats.SuperAttackDistance == ""
                    && brawlers.Stats.SuperAttackAdditionalItemName == "" && brawlers.Stats.SuperAttackAdditionalItemValue == "")
                {
                    TextBlockName1.Text = "Урон";

                    TextBlockValue1.Text = brawlers.Stats.SuperAttackValue;
                }

                else if (brawlers.Stats.SuperAttackValue == "" && brawlers.Stats.SuperAttackDistance != ""
                    && brawlers.Stats.SuperAttackAdditionalItemName == "" && brawlers.Stats.SuperAttackAdditionalItemValue == "")
                {
                    TextBlockName1.Text = "Дальность";

                    TextBlockValue1.Text = brawlers.Stats.SuperAttackDistance;
                }

                else if (brawlers.Stats.SuperAttackValue == "" && brawlers.Stats.SuperAttackDistance == ""
                    && brawlers.Stats.SuperAttackAdditionalItemName != "" && brawlers.Stats.SuperAttackAdditionalItemValue != "")
                {
                    TextBlockName1.Text = brawlers.Stats.SuperAttackAdditionalItemName;

                    TextBlockValue1.Text = brawlers.Stats.SuperAttackAdditionalItemValue;
                }
            }

            else
            {
                if (brawlers.Stats.CompanionHealth != "" && brawlers.Stats.CompanionAttack != ""
                    && brawlers.Stats.CompanionDistance != "" && brawlers.Stats.CompanionSpeed != "")
                {
                    TextBlockName1.Text = "Здоровье";
                    TextBlockName2.Text = "Скорость движения";
                    TextBlockName3.Text = "Урон";
                    TextBlockName4.Text = "Дальность";

                    TextBlockValue1.Text = brawlers.Stats.CompanionHealth;
                    TextBlockValue2.Text = brawlers.Stats.CompanionSpeed;
                    TextBlockValue3.Text = brawlers.Stats.CompanionAttack;
                    TextBlockValue4.Text = brawlers.Stats.CompanionDistance;
                }

                else if (brawlers.Stats.CompanionHealth == "" && brawlers.Stats.CompanionAttack != ""
                    && brawlers.Stats.CompanionDistance != "" && brawlers.Stats.CompanionSpeed != "")
                {
                    TextBlockName1.Text = "Урон";
                    TextBlockName2.Text = "Скорость движения";
                    TextBlockName3.Text = "Дальность";

                    TextBlockValue1.Text = brawlers.Stats.CompanionAttack;
                    TextBlockValue2.Text = brawlers.Stats.CompanionSpeed;
                    TextBlockValue3.Text = brawlers.Stats.CompanionDistance;
                }

                else if (brawlers.Stats.CompanionHealth != "" && brawlers.Stats.CompanionAttack == ""
                    && brawlers.Stats.CompanionDistance != "" && brawlers.Stats.CompanionSpeed != "")
                {
                    TextBlockName1.Text = "Здоровье";
                    TextBlockName2.Text = "Скорость движения";
                    TextBlockName3.Text = "Дальность";

                    TextBlockValue1.Text = brawlers.Stats.CompanionHealth;
                    TextBlockValue2.Text = brawlers.Stats.CompanionSpeed;
                    TextBlockValue3.Text = brawlers.Stats.CompanionDistance;
                }

                else if (brawlers.Stats.CompanionHealth != "" && brawlers.Stats.CompanionAttack != ""
                    && brawlers.Stats.CompanionDistance == "" && brawlers.Stats.CompanionSpeed != "")
                {
                    TextBlockName1.Text = "Здоровье";
                    TextBlockName2.Text = "Урон";
                    TextBlockName3.Text = "Скорость движения";

                    TextBlockValue1.Text = brawlers.Stats.CompanionHealth;
                    TextBlockValue2.Text = brawlers.Stats.CompanionAttack;
                    TextBlockValue3.Text = brawlers.Stats.CompanionSpeed;
                }

                else if (brawlers.Stats.CompanionHealth != "" && brawlers.Stats.CompanionAttack != ""
                    && brawlers.Stats.CompanionDistance != "" && brawlers.Stats.CompanionSpeed == "")
                {
                    TextBlockName1.Text = "Здоровье";
                    TextBlockName2.Text = "Урон";
                    TextBlockName3.Text = "Дальность";

                    TextBlockValue1.Text = brawlers.Stats.CompanionHealth;
                    TextBlockValue2.Text = brawlers.Stats.CompanionAttack;
                    TextBlockValue3.Text = brawlers.Stats.CompanionDistance;
                }

                else if (brawlers.Stats.CompanionHealth != "" && brawlers.Stats.CompanionAttack != ""
                    && brawlers.Stats.CompanionDistance == "" && brawlers.Stats.CompanionSpeed == "")
                {
                    TextBlockName1.Text = "Здоровье";
                    TextBlockName2.Text = "Урон";

                    TextBlockValue1.Text = brawlers.Stats.CompanionHealth;
                    TextBlockValue2.Text = brawlers.Stats.CompanionAttack;
                }

                else if (brawlers.Stats.CompanionHealth != "" && brawlers.Stats.CompanionAttack == ""
                    && brawlers.Stats.CompanionDistance != "" && brawlers.Stats.CompanionSpeed == "")
                {
                    TextBlockName1.Text = "Здоровье";
                    TextBlockName2.Text = "Дальность";

                    TextBlockValue1.Text = brawlers.Stats.CompanionHealth;
                    TextBlockValue2.Text = brawlers.Stats.CompanionDistance;
                }

                else if (brawlers.Stats.CompanionHealth != "" && brawlers.Stats.CompanionAttack == ""
                    && brawlers.Stats.CompanionDistance == "" && brawlers.Stats.CompanionSpeed != "")
                {
                    TextBlockName1.Text = "Здоровье";
                    TextBlockName2.Text = "Скорость движения";

                    TextBlockValue1.Text = brawlers.Stats.CompanionHealth;
                    TextBlockValue2.Text = brawlers.Stats.CompanionSpeed;
                }

                else if (brawlers.Stats.CompanionHealth == "" && brawlers.Stats.CompanionAttack != ""
                    && brawlers.Stats.CompanionDistance != "" && brawlers.Stats.CompanionSpeed == "")
                {
                    TextBlockName1.Text = "Урон";
                    TextBlockName2.Text = "Дальность";

                    TextBlockValue1.Text = brawlers.Stats.CompanionAttack;
                    TextBlockValue2.Text = brawlers.Stats.CompanionDistance;
                }

                else if (brawlers.Stats.CompanionHealth == "" && brawlers.Stats.CompanionAttack != ""
                    && brawlers.Stats.CompanionDistance == "" && brawlers.Stats.CompanionSpeed != "")
                {
                    TextBlockName1.Text = "Урон";
                    TextBlockName2.Text = "Скорость движения";

                    TextBlockValue1.Text = brawlers.Stats.CompanionAttack;
                    TextBlockValue2.Text = brawlers.Stats.CompanionSpeed;
                }

                else if (brawlers.Stats.CompanionHealth == "" && brawlers.Stats.CompanionAttack == ""
                    && brawlers.Stats.CompanionDistance != "" && brawlers.Stats.CompanionSpeed != "")
                {
                    TextBlockName1.Text = "Дальность";
                    TextBlockName2.Text = "Скорость движения";

                    TextBlockValue1.Text = brawlers.Stats.CompanionDistance;
                    TextBlockValue2.Text = brawlers.Stats.CompanionSpeed;
                }

                else if (brawlers.Stats.CompanionHealth != "" && brawlers.Stats.CompanionAttack == ""
                    && brawlers.Stats.CompanionDistance == "" && brawlers.Stats.CompanionSpeed == "")
                {
                    TextBlockName1.Text = "Здоровье";

                    TextBlockValue1.Text = brawlers.Stats.CompanionHealth;
                }

                else if (brawlers.Stats.CompanionHealth == "" && brawlers.Stats.CompanionAttack != ""
                    && brawlers.Stats.CompanionDistance == "" && brawlers.Stats.CompanionSpeed == "")
                {
                    TextBlockName1.Text = "Урон";

                    TextBlockValue1.Text = brawlers.Stats.CompanionAttack;
                }

                else if (brawlers.Stats.CompanionHealth == "" && brawlers.Stats.CompanionAttack == ""
                    && brawlers.Stats.CompanionDistance != "" && brawlers.Stats.CompanionSpeed == "")
                {
                    TextBlockName1.Text = "Дальность";

                    TextBlockValue1.Text = brawlers.Stats.CompanionDistance;
                }


                else if (brawlers.Stats.CompanionHealth == "" && brawlers.Stats.CompanionAttack == ""
                    && brawlers.Stats.CompanionDistance == "" && brawlers.Stats.CompanionSpeed != "")
                {
                    TextBlockName1.Text = "Скорость движения";

                    TextBlockValue1.Text = brawlers.Stats.CompanionSpeed;
                }
            }

            if (brawlers.Stats.Feature != "")
            {
                BlockF.Visibility = Visibility.Visible;
            }

            UpdateBlock(Block0Name, Block0Value, Block0);
            UpdateBlock(TextBlockName1, TextBlockValue1, Block1);
            UpdateBlock(TextBlockName2, TextBlockValue2, Block2);
            UpdateBlock(TextBlockName3, TextBlockValue3, Block3);
            UpdateBlock(TextBlockName4, TextBlockValue4, Block4);

        }

        private void UpdateBlock(TextBlock name, TextBlock value, Border block)
        {
            if (name.Text != "" && value.Text != "")
            {
                if (name.Text.Length > 19 && name.Text.Length < 22)
                {
                    name.FontSize = 18;
                    name.Margin = new Thickness(name.Margin.Left, name.Margin.Top + 4, name.Margin.Right, name.Margin.Bottom);
                }

                else if (name.Text.Length >= 22 && name.Text.Length < 25)
                {
                    name.FontSize = 16;
                    name.Margin = new Thickness(name.Margin.Left, name.Margin.Top + 6, name.Margin.Right, name.Margin.Bottom);
                }

                if (name.Text.Length >= 25)
                {
                    name.FontSize = 14;
                    name.Margin = new Thickness(name.Margin.Left, name.Margin.Top + 8, name.Margin.Right, name.Margin.Bottom);
                }

                block.Visibility = Visibility.Visible;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Keyboard.Focus(this);
            }));
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
