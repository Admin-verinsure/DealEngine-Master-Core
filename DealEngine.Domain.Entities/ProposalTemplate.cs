using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class ProposalTemplate : EntityBase, IAggregateRoot
    {
        protected ProposalTemplate() : base (null) { }

        public ProposalTemplate(User createdBy, Owner owner, string name, bool isPrivate)
			: base (createdBy)
        {
            Name = name;
            IsPrivate = isPrivate;
            Owner = owner;

            AddSection("Main Section");
        }

        private IList<Section> sections = new List<Section>();

        public virtual string Name { get; protected set; }

        public virtual bool IsPrivate { get; protected set; }

        public virtual IEnumerable<Section> Sections { get { return sections; } }

        public virtual Owner Owner { get; protected set; }

        public virtual void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name", "Name can not be null, empty or white space.");

                Name = name;
        }

        public virtual void AddSection(string sectionName)
        {
            sections.Add(new Section(CreatedBy, sectionName));
        }

        public virtual void SetVisibility(bool isPrivate)
        {
            IsPrivate = isPrivate;
        }

    }

    public class Section : EntityBase
    {
        protected Section() : base (null) { }

        public Section(User createdBy, string sectionName)
			: base (createdBy)
        {            
            Name = sectionName;
        }

        IList<Question> questions = new List<Question>();

        public virtual string Name { get; protected set; }

        public virtual IEnumerable<Question> Questions { get { return questions; } }

        public virtual void AddNewQuestion(string questionName)
        {
            questions.Add(new Question(CreatedBy, questionName));
        }

        public virtual void AddExistingQuestion(Question question)
        {
            questions.Add(question);
        }

        public virtual void RemoveQuestion(Question question)
        {
            questions.Remove(question);
        }
    }

    public class Question : EntityBase
    {
        protected Question() : base (null) { }

        public Question (User createdBy, string questionLabel)
			: base (createdBy)
        {
            Label = questionLabel;
        }

        public virtual string Label { get; protected set; }
    } 
}
