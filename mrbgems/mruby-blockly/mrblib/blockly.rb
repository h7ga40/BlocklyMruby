# Porting from 
# https://github.com/jeanlazarou/blockly2ruby
# Copyright (c) 2014 Jean Lazarou
# MIT Lisence

def blockly_puts x
  if x.is_a?(Array)
    puts x.join(',')
  else
    puts x
  end
end

class Array
  def find_first v
    i = self.index(v)
    i ? i + 1 : 0
  end

  def find_last v
    i = self.rindex(v)
    i ? i + 1 : 0
  end

  def numbers
    self.delete_if {|v| !v.is_a?(Numeric)}
  end

  def sum
    self.numbers.inject(0) {|sum, v| sum + v}
  end

  def average
    x = self.numbers
    x.sum / x.size.to_f
  end

  def standard_deviation
    x = self.numbers
    return 0 if x.empty?
    mean = x.average
    variance = x.map {|v| (v - mean) ** 2}.sum / x.size
    Math.sqrt(variance)
  end

  def median
    x = self.numbers
    x.sort!
    index  = x.size / 2
    x.size.odd? ? x[index] : ((x[index - 1] + x[index]) / 2.0)
  end
end

class String
  def find_first v
    i = self.index(v)
    i ? i + 1 : 0
  end

  def find_last v
    i = self.rindex(v)
    i ? i + 1 : 0
  end
end

class Float
  def even?
    false
  end

  def odd?
    false
  end
end

def colour_rgb(r, g, b)
  r = (2.55 * [100, [0, r].max].min).round
  g = (2.55 * [100, [0, g].max].min).round
  b = (2.55 * [100, [0, b].max].min).round
  '#%02x%02x%02x' % [r, g, b]
end

def colour_blend(colour1, colour2, ratio)
  _, r1, g1, b1 = colour1.unpack('A1A2A2A2').map {|x| x.to_i(16)}
  _, r2, g2, b2 = colour2.unpack('A1A2A2A2').map {|x| x.to_i(16)}
  ratio = [1, [0, ratio].max].min
  r = (r1 * (1 - ratio) + r2 * ratio).round
  g = (g1 * (1 - ratio) + g2 * ratio).round
  b = (b1 * (1 - ratio) + b2 * ratio).round
  '#%02x%02x%02x' % [r, g, b]
end

def lists_random_item(myList)
  myList[rand(myList.size)]
end

def lists_remove_random_item(myList)
  myList.delete_at(rand(myList.size))
end

def lists_sublist(myList, range)
  myList[range] || []
end

def lists_insert_random_item(myList, value)
  myList.insert(rand(myList.size), value)
end

def lists_set_random_item(myList, value)
  myList[rand(myList.size)] = value
end

# loops though all numbers from +params[:from]+ to +params[:to]+ by the step
# value +params[:by]+ and calls the given block passing the numbers
def for_loop params
  from = params[:from] #.to_f
  to = params[:to] #.to_f
  by = params[:by].abs #.to_f

  from.step(to, (from > to) ? -1 * by : by) do |value|
    yield value
  end
end

def is_prime n
  return false if n < 0
  (2..Math.sqrt(n)).each { |i| return false if n % i == 0}
  true
end

def math_modes(some_list)
  groups = some_list.group_by{|v| v}
  groups = groups.sort {|a,b| b[1].size <=> a[1].size}
  max_size = groups[0][1].size
  modes = []

  groups.each do |group|
    break if group[1].size != max_size
    modes << group[0]
  end

  modes
end

def text_get_from_start(text, index)
  return "" if index < 0
  text[index] || ""
end

def text_get_from_end(text, index)
  return "" if index < 0
  text[-index-1] || ""
end

def text_random_letter(text)
  text[rand(text.size)]
end

def text_to_title_case(str)
  str.gsub(/\S+/) {|txt| txt.capitalize}
end

def text_prompt(msg)
  print(msg)
  $stdin.gets
end
